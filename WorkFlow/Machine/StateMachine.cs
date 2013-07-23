using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.ComponentModel;
using System.Activities;
using System.Activities.Expressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;


namespace Machine
{
    [ContentProperty("States")]
   // [System.ComponentModel.Designer(typeof(Machine.Design.StateMachineDesigner))]
    public sealed class StateMachine : NativeActivity
    {
        //states in root level of StateMachine
        Collection<State> states;
        //variables used in StateMachine
        Collection<Variable> variables;

        //internal representations of states
        Collection<InternalState> internalStates;
        //ActivityFuncs who call internal activities
        Collection<ActivityFunc<string, StateMachineEventManager, string>> internalStateFuncs;

        //Callback when a state completes
        CompletionCallback<string> onStateComplete;
        //eventManager is used to manage the events of trigger completion.
        //When a trigger on a transition is completed, the corresponding event will be sent to eventManager.
        //eventManager will decide whether immediate process it or just register it.
        Variable<StateMachineEventManager> eventManager;

        //internal Id of StateMachine. it's a constant value and states of state machine will generate their ids based on this root id.
        const string rootId = "0";

        /// <summary>
        /// It's constructor.
        /// </summary>
        public StateMachine()
        {
            this.internalStates = new Collection<InternalState>();
            this.internalStateFuncs = new Collection<ActivityFunc<string, StateMachineEventManager, string>>();
            this.eventManager = new Variable<StateMachineEventManager> { Name = "EventManager", Default = new LambdaValue<StateMachineEventManager>(ctx => new StateMachineEventManager()) };
            this.onStateComplete = new CompletionCallback<string>(OnStateComplete);
        }

        /// <summary>
        /// It represents the start point of the StateMachine.
        /// </summary>
        [DefaultValue(null)]
        public State InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// It contains all root level States in the StateMachine.
        /// </summary>
        [DependsOn("InitialState")]
        public Collection<State> States
        {
            get
            {
                if (this.states == null)
                {
                    this.states = new ValidatingCollection<State>
                    {

                        // disallow null values
                        OnAddValidationCallback = item =>
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException("item");
                            }
                        },
                    };
                }
                return this.states;

            }
        }

        /// <summary>
        /// It contains Variables which can be used within StateMachine scope.
        /// </summary>
        [DependsOn("States")]
        public Collection<Variable> Variables
        {
            get
            {
                if (this.variables == null)
                {
                    this.variables = new ValidatingCollection<Variable>
                    {

                        // disallow null values
                        OnAddValidationCallback = item =>
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException("item");
                            }
                        },
                    };
                }
                return this.variables;
            }
        }

        /// <summary>
        /// Do validation and if validation is successful, create internal representations for states and transitions.
        /// </summary>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            //cleanup
            this.internalStateFuncs.Clear();
            this.internalStates.Clear();

            //clear Ids and Flags via transitions
            this.PassNumber++;
            TraverseViaTransitions(ClearState, ClearTransition);
            //clear Ids and Flags via children
            this.PassNumber++;
            TraverseViaChildren(ClearStates, null, ClearTransitions, false);

            //Mark via children and do some check
            this.PassNumber++;
            TraverseViaChildren(MarkStatesViaChildren,
                delegate(State state) { StateCannotHaveMultipleParents(metadata, state); }, null, false);

            this.PassNumber++;
            //Mark via transition
            TraverseViaTransitions(delegate(State state) { MarkStateViaTransition(metadata, state); }, null);

            //Do validation via children
            //need not check the violation of state which is not reached
            this.PassNumber++;
            TraverseViaChildren(delegate(Collection<State> states) { ValidateStates(metadata, states); }, null,
                delegate(State state) { ValidateTransitions(metadata, state); }, true);

            //Validate the root state machine itself
            ValidateStateMachine(metadata);

            if (!metadata.HasViolations)
            {
                StateMachineHelper.ProcessChildStates(metadata, this.States, this.internalStates, this.internalStateFuncs);
            }
            metadata.AddImplementationVariable(this.eventManager);
            foreach (Variable variable in this.Variables)
            {
                metadata.AddVariable(variable);
            }
        }

        /// <summary>
        /// Execution of StateMachine
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                        Justification = "The context is used by workflow runtime. The parameter should be fine.")]
        protected override void Execute(NativeActivityContext context)
        {
            //We view the duration before moving to initial state is on transition.
            StateMachineEventManager localEventManager = this.eventManager.Get(context);
            localEventManager.OnTransition = true;
            int index = StateMachineHelper.GetChildStateIndex(rootId, this.InitialState.StateId);
            context.ScheduleFunc<string, StateMachineEventManager, string>(this.internalStateFuncs[index], this.InitialState.StateId,
                localEventManager, onStateComplete);
        }

        void OnStateComplete(NativeActivityContext context, ActivityInstance completedInstance, string result)
        {
            if (StateMachineHelper.IsAncestor(rootId, result))
            {
                int index = StateMachineHelper.GetChildStateIndex(rootId, result);
                context.ScheduleFunc<string, StateMachineEventManager, string>(this.internalStateFuncs[index], result,
                this.eventManager.Get(context), onStateComplete);
            }
        }

        void ValidateStateMachine(NativeActivityMetadata metadata)
        {
            if (this.InitialState == null)
            {
                metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.StateMachineMustHaveInitialState, this.DisplayName));
            }
            else
            {
                if (this.InitialState.IsFinal)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.InitialStateCannotBeFinalState, this.InitialState.DisplayName));
                }
                if (this.InitialState.States.Count > 0)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.InitialStateMustBeSimpleState, this.InitialState.DisplayName));
                }
            }
        }

        uint PassNumber
        {
            get;
            set;
        }

        void TraverseViaChildren(Action<Collection<State>> actionForStates, Action<State> faultActionForState, Action<State> actionForTransitions, bool checkReached)
        {
            if (actionForStates != null)
            {
                actionForStates(this.States);
            }

            uint passNumber = this.PassNumber;
            Stack<State> stack = new Stack<State>();
            foreach (State state in this.States)
            {
                stack.Push(state);
            }

            while (stack.Count > 0)
            {
                State state = stack.Pop();
                if (state.PassNumber != passNumber)
                {
                    if (!checkReached || state.Reachable)
                    {
                        state.PassNumber = passNumber;
                        if (actionForStates != null)
                        {
                            actionForStates(state.States);
                        }
                        if (actionForTransitions != null)
                        {
                            actionForTransitions(state);
                        }
                        foreach (State childState in state.States)
                        {
                            childState.ParentState = state;
                            stack.Push(childState);
                        }
                    }
                }
                else if (faultActionForState != null)
                {
                    faultActionForState(state);
                }

            }
        }

        static void MarkStatesViaChildren(Collection<State> states)
        {
            if (states.Count > 0)
            {
                State parent = states[0].ParentState;
                for (int i = 0; i < states.Count; i++)
                {
                    State state = states[i];
                    if (parent == null) //root level states
                    {
                        state.StateId = GenerateStateId(rootId, i);
                        state.HasInheritedTransition = false;
                    }
                    else
                    {
                        state.StateId = GenerateStateId(parent.StateId, i);
                        state.HasInheritedTransition = parent.HasInheritedTransition || parent.Transitions.Count > 0;
                    }

                }
            }

        }

        static void StateCannotHaveMultipleParents(NativeActivityMetadata metadata, State state)
        {
            metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.StateCannotHaveMultipleParents, state.DisplayName));
        }


        static void MarkStateViaTransition(NativeActivityMetadata metadata, State state)
        {
            if (string.IsNullOrEmpty(state.StateId))
            {
                metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.StateNotBelongToAnyParent, state.DisplayName));
            }
            state.Reachable = true;
        }

        void TraverseViaTransitions(Action<State> actionForState, Action<Transition> actionForTransition)
        {
            Stack<State> stack = new Stack<State>();
            stack.Push(this.InitialState);
            uint passNumber = this.PassNumber;
            while (stack.Count > 0)
            {
                State currentState = stack.Pop();
                if (currentState == null || currentState.PassNumber == passNumber)
                {
                    continue;
                }
                currentState.PassNumber = passNumber;
                if (actionForState != null)
                {
                    actionForState(currentState);
                }
                foreach (Transition transition in currentState.Transitions)
                {
                    if (actionForTransition != null)
                    {
                        actionForTransition(transition);
                    }
                    stack.Push(transition.To);
                }
                State parentState = currentState.ParentState;
                while (parentState != null && parentState.PassNumber != passNumber)
                {
                    stack.Push(parentState);
                    parentState = parentState.ParentState;
                }
            }
        }

        static void ClearStates(Collection<State> states)
        {
            foreach (State state in states)
            {
                ClearState(state);
            }
        }

        static void ClearState(State state)
        {
            state.ParentState = null;
            state.StateId = null;
            state.Reachable = false;
            state.ClearInternalState();
            state.HasInheritedTransition = false;
        }

        static void ClearTransitions(State state)
        {
            foreach (Transition transition in state.Transitions)
            {
                ClearTransition(transition);
            }
        }

        static void ClearTransition(Transition transition)
        {
            transition.Source = null;
        }

        static void ValidateStates(NativeActivityMetadata metadata, Collection<State> states)
        {
            foreach (State state in states)
            {
                //only validate reached state.
                ValidateState(metadata, state);
            }
        }

        static void ValidateState(NativeActivityMetadata metadata, State state)
        {
            if (state.IsFinal)
            {
                if (state.Entry != null)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.FinalStateCannotHaveEntry, state.DisplayName));
                }
                if (state.Exit != null)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.FinalStateCannotHaveExit, state.DisplayName));
                }
                if (state.States.Count > 0)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.FinalStateCannotHaveChildState, state.DisplayName));
                }
                if (state.Transitions.Count > 0)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.FinalStateCannotHaveTransition, state.DisplayName));
                }
                if (state.ParentState != null)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.FinalStateCanOnlyBeOnRootLevel, state.DisplayName, state.ParentState.DisplayName));
                }
            }
            else
            {
                if (state.States.Count == 0 && !state.HasInheritedTransition && state.Transitions.Count == 0)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.SimpleStateWithoutInheritedTransitionMustHaveTransition, state.DisplayName));
                }
            }
        }

        static void ValidateTransitions(NativeActivityMetadata metadata, State currentState)
        {
            Collection<Transition> transitions = currentState.Transitions;
            HashSet<Activity> conditionalTransitionTriggers = new HashSet<Activity>();
            Dictionary<Activity, List<Transition>> unconditionalTransitionMapping = new Dictionary<Activity, List<Transition>>();

            foreach (Transition transition in transitions)
            {
                if (transition.Source != null)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.TransitionCannotBeAddedTwice, transition.DisplayName, currentState.DisplayName, transition.Source.DisplayName));
                    continue;
                }
                else
                {
                    transition.Source = currentState;
                }

                if (transition.To == null)
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.TransitionTargetCannotBeNull, transition.DisplayName, currentState.DisplayName));
                }
                else
                {
                    if (transition.To.States.Count > 0)
                    {
                        metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.TargetStateMustBeSimpleState, transition.To.DisplayName, transition.DisplayName));
                    }

                    if (StateMachineHelper.IsAncestor(currentState.StateId, transition.To.StateId))
                    {
                        metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.AncestorCannotHaveTransitionToDescendant, currentState.DisplayName, transition.DisplayName, transition.To.DisplayName));
                    }
                }

                if (transition.Trigger != null)
                {
                    if (transition.Condition == null)
                    {
                        if (!unconditionalTransitionMapping.ContainsKey(transition.Trigger))
                        {
                            unconditionalTransitionMapping.Add(transition.Trigger, new List<Transition>());
                        }
                        unconditionalTransitionMapping[transition.Trigger].Add(transition);
                    }
                    else
                    {
                        conditionalTransitionTriggers.Add(transition.Trigger);
                    }
                }
                else
                {
                    metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.TriggerCannotBeNull, transition.DisplayName, currentState.DisplayName));
                }

            }
            foreach (KeyValuePair<Activity, List<Transition>> unconditionalTransitions in unconditionalTransitionMapping)
            {
                if (conditionalTransitionTriggers.Contains(unconditionalTransitions.Key) || unconditionalTransitions.Value.Count > 1)
                {
                    foreach (Transition transition in unconditionalTransitions.Value)
                    {
                        metadata.AddValidationError(string.Format(CultureInfo.CurrentCulture, SR.UnconditionalTransitionShouldnotShareTriggersWithOthers, transition.DisplayName,
                                   currentState.DisplayName, transition.Trigger.DisplayName));
                    }
                };
            }
        }

        static string GenerateStateId(string parentId, int index)
        {
            return parentId + "_" + index.ToString(CultureInfo.InvariantCulture);
        }
    }
}
