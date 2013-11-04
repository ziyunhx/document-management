using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Activities;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Machine
{
  public sealed class InternalState : NativeActivity<string>
    {
        /// State denotes corresponding State object.
        State state;
        //internal representation of child states
        Collection<InternalState> internalStates;
        //ActivityFunc of internal child states
        Collection<ActivityFunc<string, StateMachineEventManager, string>> internalStateFuncs;
        //internal representation of transitions.
        Collection<InternalTransition> internalTransitions;

        //number of running triggers
        Variable<int> currentRunningTriggers;
        Variable<bool> hasRunningChildState;
        Variable<bool> isExiting;
        //This bookmark is used to evaluate condition of a transition of this state. 
        Variable<Bookmark> evaluateConditionBookmark;
        Variable<Bookmark> completeStateBookmark;


        //Callback which is called when Entry is completed.
        CompletionCallback onEntryComplete;
        //Callback which is called when Trigger is completed.
        CompletionCallback onTriggerComplete;
        //Callback which is called when Condition is completed.
        CompletionCallback<bool> onConditionComplete;
        //Callback which is called when Exit is completed.
        CompletionCallback onExitComplete;
        //Callback which is called when child state is completed.
        CompletionCallback<string> onChildStateComplete;
        //Callback which is used to complete this state by one of its ancestors.
        BookmarkCallback completeStateCallback;
        //Callback which is used to start to evaluate Condition of a transition of this state.
        BookmarkCallback evaluateConditionCallback;

        Dictionary<Activity, InternalTransition> triggerInternalTransitionMapping = new Dictionary<Activity, InternalTransition>();

        public InternalState(State state)
        {
            this.state = state;
            this.DisplayName = state.DisplayName;

            this.onEntryComplete = new CompletionCallback(OnEntryComplete);
            this.onTriggerComplete = new CompletionCallback(OnTriggerComplete);
            this.onConditionComplete = new CompletionCallback<bool>(OnConditionComplete);
            this.onExitComplete = new CompletionCallback(OnExitComplete);
            this.onChildStateComplete = new CompletionCallback<string>(OnChildStateComplete);

            this.completeStateCallback = new BookmarkCallback(CompleteState);
            this.evaluateConditionCallback = new BookmarkCallback(StartEvaluateCondition);

            this.currentRunningTriggers = new Variable<int>();
            this.hasRunningChildState = new Variable<bool>();
            this.isExiting = new Variable<bool>();
            this.evaluateConditionBookmark = new Variable<Bookmark>();
            this.completeStateBookmark = new Variable<Bookmark>();

            this.internalStates = new Collection<InternalState>();
            this.internalTransitions = new Collection<InternalTransition>();

            this.internalStateFuncs = new Collection<ActivityFunc<string, StateMachineEventManager, string>>();
            this.triggerInternalTransitionMapping = new Dictionary<Activity, InternalTransition>();
        }

        /// <summary>
        /// ToState is target state ID.
        /// ToState ID has 2 categories: 1. the same as ID of this state 2. the same as ID of one of descendants of this state.
        /// For second case, this state will schedule its one of its child states who is the target state or ancestor of the target state.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ToState
        {
            get;
            set;
        }

        /// <summary>
        /// EventManager is used to globally manage event queue such that triggered events can be processed in order.
        /// </summary>
        [RequiredArgument]
        public InArgument<StateMachineEventManager> EventManager
        {
            get;
            set;
        }

        /// <summary>
        /// Entry if supplied will be executed when state is entering.
        /// </summary>
        public Activity Entry
        {
            get
            {
                return this.state.Entry;
            }
        }

        /// <summary>
        /// Exit if supplied will be executed when state is leaving.
        /// </summary>
        public Activity Exit
        {
            get
            {
                return this.state.Exit;
            }
        }

        /// <summary>
        /// IsFinal denotes whether this state is a final state or not.
        /// </summary>
        [DefaultValue(false)]
        public bool IsFinal
        {
            get
            {
                return this.state.IsFinal;

            }
        }

        /// <summary>
        /// StateId is identifier of a state. It's unique within a StateMachine.
        /// </summary>
        public string StateId
        {
            get
            {
                return this.state.StateId;
            }
        }

        /// <summary>
        /// States collection contains child state objects.
        /// </summary>
        public Collection<State> States
        {
            get
            {
                return this.state.States;
            }
        }

        /// <summary>
        /// Transitions collection contains transitions on this state.
        /// </summary>
        public Collection<Transition> Transitions
        {
            get
            {
                return this.state.Transitions;
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            //clean up
            this.internalStates.Clear();
            this.internalStateFuncs.Clear();
            this.internalTransitions.Clear();

            StateMachineHelper.ProcessChildStates(metadata, this.States, this.internalStates, this.internalStateFuncs);

            if (this.Entry != null)
            {
                metadata.AddChild(this.Entry);
            }
            if (this.Exit != null)
            {
                metadata.AddChild(this.Exit);
            }

            ProcessTransitions(metadata);

            metadata.AddArgument(new RuntimeArgument("ToState", this.ToState.ArgumentType, ArgumentDirection.In));
            metadata.AddArgument(new RuntimeArgument("EventManager", this.EventManager.ArgumentType, ArgumentDirection.In));

            metadata.AddImplementationVariable(this.currentRunningTriggers);
            metadata.AddImplementationVariable(this.hasRunningChildState);
            metadata.AddImplementationVariable(this.isExiting);
            metadata.AddImplementationVariable(this.evaluateConditionBookmark);
            metadata.AddImplementationVariable(this.completeStateBookmark);
        }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                        Justification = "The context is used by workflow runtime. The parameter should be fine.")]
        protected override void Execute(NativeActivityContext context)
        {
            //when a state starts, it should set its completion bookmark to event queue for later usage by its ancestors when inherited transition is taken.
            if (!this.IsFinal)
            {
                Bookmark bookmark = context.CreateBookmark(this.completeStateCallback);
                this.EventManager.Get(context).CompleteDeepestChildBookmark = bookmark;
                this.completeStateBookmark.Set(context, bookmark);
            }
            this.isExiting.Set(context, false);
            ScheduleEntry(context);
        }

        protected override void Abort(NativeActivityAbortContext context)
        {
            RemoveActiveBookmark(context);
            base.Abort(context);
        }

        protected override void Cancel(NativeActivityContext context)
        {
            RemoveActiveBookmark(context);
            base.Cancel(context);
        }

        void ScheduleEntry(NativeActivityContext context)
        {
            if (this.Entry != null)
            {
                context.ScheduleActivity(this.Entry, this.onEntryComplete);
            }
            else
            {
                onEntryComplete(context, null);
            }
        }

        void CompleteState(NativeActivityContext context, Bookmark bookmark, object value)
        {
            PrepareForExit(context, (string)value);
        }

        void ScheduleChildState(NativeActivityContext context)
        {
            //schedule child state
            string id = this.ToState.Get(context);
            if (this.StateId != id)
            {
                ScheduleChildState(context, id);
            }
            else
            {
                Debug.Assert(this.States.Count == 0, "The target state must be a simple state.");
                //We've reached the target of transition. 
                //when a transition is done, we should continue the processing of event queue.
                ProcessNextTriggerCompletedEvent(context, this.EventManager.Get(context));
            }
        }

        void OnEntryComplete(NativeActivityContext context, ActivityInstance instance)
        {
            ScheduleChildState(context);
            ScheduleTriggers(context);
        }

        void ScheduleTriggers(NativeActivityContext context)
        {
            if (!this.IsFinal)
            {
                //Final state need not condition evaluation bookmark.
                AddEvaluateConditionBookmark(context);
            }

            if (this.internalTransitions.Count > 0)
            {
                foreach (InternalTransition transition in this.internalTransitions)
                {
                    context.ScheduleActivity(transition.Trigger, this.onTriggerComplete);
                }
                this.currentRunningTriggers.Set(context, this.currentRunningTriggers.Get(context) + this.internalTransitions.Count);
            }
        }

        void OnTriggerComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            int runningTriggers = this.currentRunningTriggers.Get(context);
            this.currentRunningTriggers.Set(context, --runningTriggers);
            bool isOnExit = this.isExiting.Get(context);

            if (!context.IsCancellationRequested && runningTriggers == 0 && !this.hasRunningChildState.Get(context) && isOnExit)
            {
                ScheduleExit(context);
            }
            else if (completedInstance.State == ActivityInstanceState.Closed)
            {
                InternalTransition internalTransition = null;
                this.triggerInternalTransitionMapping.TryGetValue(completedInstance.Activity, out internalTransition);
                Debug.Assert(internalTransition != null, "internalTransition should be added into triggerInternalTransitionMapping in CacheMetadata.");

                StateMachineEventManager eventManager = this.EventManager.Get(context);
                bool canBeProcessedImmediately;
                eventManager.RegisterCompletedEvent(
                    new TriggerCompletedEvent { Bookmark = this.evaluateConditionBookmark.Get(context), TriggedId = internalTransition.InternalTransitionIndex },
                    out canBeProcessedImmediately
                    );
                if (canBeProcessedImmediately)
                {
                    StartEvaluateCondition(context);
                }
            }
        }

        void ScheduleChildState(NativeActivityContext context, string id)
        {
            int index = StateMachineHelper.GetChildStateIndex(this.StateId, id);
            Debug.Assert(index >= 0 && index < this.internalStateFuncs.Count);
            context.ScheduleFunc<string, StateMachineEventManager, string>(this.internalStateFuncs[index],
                id, this.EventManager.Get(context), this.onChildStateComplete);
            this.hasRunningChildState.Set(context, true);
        }

        void OnChildStateComplete(NativeActivityContext context, ActivityInstance instance, string targetStateId)
        {
            InternalState completedState = (InternalState)instance.Activity;
            this.hasRunningChildState.Set(context, false);
            //When child state is completed, current state become the deepest child state.
            this.EventManager.Get(context).CompleteDeepestChildBookmark = this.completeStateBookmark.Get(context);
            switch (instance.State)
            {
                case ActivityInstanceState.Closed:
                    Debug.Assert(!completedState.IsFinal, "Final state is only on root level.");

                    if (StateMachineHelper.IsAncestor(this.StateId, targetStateId))
                    {
                        ScheduleChildState(context, targetStateId);
                    }
                    else
                    {
                        //The target state does not belong to this state or the target state equals this state.
                        //In this case, this state should be completed.
                        PrepareForExit(context, targetStateId);
                    }
                    break;
                case ActivityInstanceState.Canceled:
                    if (this.currentRunningTriggers.Get(context) == 0)
                    {
                        ScheduleExit(context);
                    }
                    break;
            }
        }

        void StartEvaluateCondition(NativeActivityContext context, Bookmark bookmark, object value)
        {
            StartEvaluateCondition(context);
        }

        void StartEvaluateCondition(NativeActivityContext context)
        {
            //Start to evaluate conditions of the trigger which represented by currentTriggerIndex
            StateMachineEventManager eventManager = this.EventManager.Get(context);
            int triggerId = eventManager.CurrentBeingProcessedEvent.TriggedId;
            eventManager.CurrentConditionIndex = 0;
            InternalTransition transition = GetInternalTransition(triggerId);

            if (transition.IsUnconditional)
            {
                TakeTransition(context, eventManager, triggerId);
            }
            else
            {
                context.ScheduleActivity<bool>(GetCondition(triggerId, eventManager.CurrentConditionIndex),
                    this.onConditionComplete, null);
            }
        }

        void OnConditionComplete(NativeActivityContext context, ActivityInstance completedInstance, bool result)
        {
            StateMachineEventManager eventManager = this.EventManager.Get(context);
            int triggerId = eventManager.CurrentBeingProcessedEvent.TriggedId;

            if (result)
            {
                TakeTransition(context, eventManager, triggerId);
            }
            else
            {
                int currentConditionIndex = eventManager.CurrentConditionIndex;
                InternalTransition transition = GetInternalTransition(triggerId);
                currentConditionIndex++;
                if (currentConditionIndex < transition.TransitionDataList.Count)
                {
                    eventManager.CurrentConditionIndex = currentConditionIndex;
                    context.ScheduleActivity<bool>(transition.TransitionDataList[currentConditionIndex].Condition, this.onConditionComplete, null);
                }
                else
                {
                    //Schedule current trigger again firstly.
                    context.ScheduleActivity(transition.Trigger, onTriggerComplete);
                    this.currentRunningTriggers.Set(context, this.currentRunningTriggers.Get(context) + 1);

                    //check whether there is any other trigger completed.
                    ProcessNextTriggerCompletedEvent(context, eventManager);
                }
            }
        }

        void ScheduleExit(NativeActivityContext context)
        {
            if (this.Exit != null)
            {
                context.ScheduleActivity(this.Exit, this.onExitComplete);
            }
            else
            {
                onExitComplete(context, null);
            }
        }

        void OnExitComplete(NativeActivityContext context, ActivityInstance instance)
        {
            ScheduleAction(context);
        }

        void ScheduleAction(NativeActivityContext context)
        {
            StateMachineEventManager eventManager = this.EventManager.Get(context);
            if (eventManager.IsReferredByBeingProcessedEvent(this.evaluateConditionBookmark.Get(context)))
            {
                InternalTransition transition = GetInternalTransition(eventManager.CurrentBeingProcessedEvent.TriggedId);
                Activity action = transition.TransitionDataList[eventManager.CurrentConditionIndex].Action;
                if (action != null)
                {
                    context.ScheduleActivity(action);
                }
            }
            RemoveBookmarks(context);
        }

        static void ProcessNextTriggerCompletedEvent(NativeActivityContext context, StateMachineEventManager eventManager)
        {
            eventManager.CurrentBeingProcessedEvent = null;
            eventManager.OnTransition = false;

            TriggerCompletedEvent completedEvent = eventManager.GetNextCompletedEvent();
            if (completedEvent != null)
            {
                context.ResumeBookmark(completedEvent.Bookmark, null);
            }
        }

        void ProcessTransitions(NativeActivityMetadata metadata)
        {
            for (int i = 0; i < this.Transitions.Count; i++)
            {
                Transition transition = this.Transitions[i];
                InternalTransition internalTransition = null;
                if (!triggerInternalTransitionMapping.TryGetValue(transition.Trigger, out internalTransition))
                {
                    metadata.AddChild(transition.Trigger);
                    internalTransition = new InternalTransition
                    {
                        Trigger = transition.Trigger,
                        InternalTransitionIndex = this.internalTransitions.Count,
                    };
                    triggerInternalTransitionMapping.Add(transition.Trigger, internalTransition);
                    this.internalTransitions.Add(internalTransition);
                }
                AddTransitionData(metadata, internalTransition, transition);
            }
        }

        static void AddTransitionData(NativeActivityMetadata metadata, InternalTransition internalTransition, Transition transition)
        {
            TransitionData transitionData = new TransitionData();
            Activity<bool> condition = transition.Condition;
            transitionData.Condition = condition;
            if (condition != null)
            {
                metadata.AddChild(condition);
            }

            Activity action = transition.Action;
            transitionData.Action = action;
            if (action != null)
            {
                metadata.AddChild(action);
            }

            if (transition.To != null)
            {
                transitionData.To = transition.To.InternalState;
            }
            internalTransition.TransitionDataList.Add(transitionData);
        }

        InternalTransition GetInternalTransition(int triggerIndex)
        {
            return this.internalTransitions[triggerIndex];
        }

        Activity<bool> GetCondition(int triggerIndex, int conditionIndex)
        {
            return this.internalTransitions[triggerIndex].TransitionDataList[conditionIndex].Condition;
        }

        string GetTo(int triggerIndex, int conditionIndex)
        {
            return this.internalTransitions[triggerIndex].TransitionDataList[conditionIndex].To.StateId;
        }

        void AddEvaluateConditionBookmark(NativeActivityContext context)
        {
            Bookmark bookmark = context.CreateBookmark(this.evaluateConditionCallback, BookmarkOptions.MultipleResume);
            this.evaluateConditionBookmark.Set(context, bookmark);
            this.EventManager.Get(context).AddActiveBookmark(bookmark);
        }

        void RemoveBookmarks(NativeActivityContext context)
        {
            context.RemoveAllBookmarks();
            RemoveActiveBookmark(context);
        }

        void RemoveActiveBookmark(ActivityContext context)
        {
            StateMachineEventManager eventManager = this.EventManager.Get(context);
            Bookmark bookmark = this.evaluateConditionBookmark.Get(context);
            if (bookmark != null)
            {
                eventManager.RemoveActiveBookmark(bookmark);
            }

        }

        void TakeTransition(NativeActivityContext context, StateMachineEventManager eventManager, int triggerId)
        {
            this.EventManager.Get(context).OnTransition = true;
            if (!this.hasRunningChildState.Get(context))
            {
                PrepareForExit(context, GetTo(triggerId, eventManager.CurrentConditionIndex));
            }
            else
            {
                context.ResumeBookmark(this.EventManager.Get(context).CompleteDeepestChildBookmark, GetTo(triggerId, eventManager.CurrentConditionIndex));
            }
        }

        void PrepareForExit(NativeActivityContext context, string targetStateId)
        {
            Debug.Assert(!this.hasRunningChildState.Get(context), "There should not be any running child state.");
            ReadOnlyCollection<ActivityInstance> children = context.GetChildren();
            this.Result.Set(context, targetStateId);
            this.isExiting.Set(context, true);
            if (children.Count > 0)
            {
                // Cancel all other pending triggers.
                context.CancelChildren();
            }
            else
            {
                ScheduleExit(context);
            }
        }
    }
}
