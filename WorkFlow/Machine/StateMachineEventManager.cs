using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Activities;
    using System.Runtime.Serialization;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Globalization;

    /// <summary>
    /// StateMachineEventManager is used to manage triggered events globally.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "This type is actually used in LINQ expression and FxCop didn't detect that.")]
    [DataContract]
   public class StateMachineEventManager
    {
        //To avoid out of memory, set a fixed length of event queue.
        const int MaxQueueLength = 1 << 25;
        //queue is used to store triggered events
        [DataMember(EmitDefaultValue = false)]
        Queue<TriggerCompletedEvent> queue;
        //If a state is running, its condition evaluation bookmark will be added in to activityBookmarks.
        //If a state is completed, its bookmark will be removed.
        [DataMember(EmitDefaultValue = false)]
        Collection<Bookmark> activeBookmarks;

        /// <summary>
        /// Constructor to do initialization.
        /// </summary>
        public StateMachineEventManager()
        {
            this.queue = new Queue<TriggerCompletedEvent>();
            this.activeBookmarks = new Collection<Bookmark>();
        }

        /// <summary>
        /// Register a completed event and returns whether the event could be processed immediately.
        /// </summary>
        public void RegisterCompletedEvent(TriggerCompletedEvent completedEvent, out bool canBeProcessedImmediately)
        {
            if (CanProcessEventImmediately)
            {
                this.CurrentBeingProcessedEvent = completedEvent;
                canBeProcessedImmediately = true;
                return;
            }
            else if (queue.Count < MaxQueueLength)
            {
                this.queue.Enqueue(completedEvent);
                canBeProcessedImmediately = false;
                return;
            }
            canBeProcessedImmediately = false;
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.TooManyCompletedEvents, MaxQueueLength));
        }

        /// <summary>
        /// Get next completed events queue.
        /// </summary>
        public TriggerCompletedEvent GetNextCompletedEvent()
        {
            while (this.queue.Count > 0)
            {
                TriggerCompletedEvent completedEvent = this.queue.Dequeue();
                if (this.activeBookmarks.Contains(completedEvent.Bookmark))
                {
                    this.CurrentBeingProcessedEvent = completedEvent;
                    return completedEvent;
                }
            }
            return null;
        }

        /// <summary>
        /// If an event is being processed, CurrentConditionIndex denotes the index of condition is being evaluated.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int CurrentConditionIndex
        {
            get;
            set;
        }

        /// <summary>
        /// CurrentBeingProcessedEvent denotes trigger index of current being processed event.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public TriggerCompletedEvent CurrentBeingProcessedEvent
        {
            get;
            set;
        }

        /// <summary>
        /// This bookmark is used to complete deepest running state.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Bookmark CompleteDeepestChildBookmark
        {
            get;
            set;
        }

        /// <summary>
        /// OnTransition denotes whether StateMachine is on the way of transition.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool OnTransition
        {
            get;
            set;
        }

        /// <summary>
        /// When StateMachine enters a state, condition evaluation bookmark of that state would be added to activeBookmarks collection.
        /// </summary>
        public void AddActiveBookmark(Bookmark bookmark)
        {
            this.activeBookmarks.Add(bookmark);
        }

        /// <summary>
        /// When StateMachine leaves a state, condition evaluation bookmark of that state would be removed from activeBookmarks collection.
        /// </summary>
        public void RemoveActiveBookmark(Bookmark bookmark)
        {
            this.activeBookmarks.Remove(bookmark);
        }

        /// <summary>
        /// This method is used to denote whether a given bookmark is referred by currently processed event.
        /// </summary>
        public bool IsReferredByBeingProcessedEvent(Bookmark bookmark)
        {
            return this.CurrentBeingProcessedEvent != null && this.CurrentBeingProcessedEvent.Bookmark == bookmark;
        }

        /// <summary>
        /// CanProcessEventImmediately denotes whether StateMachineManger is ready to process an event immediately.
        /// </summary>
        bool CanProcessEventImmediately
        {
            get
            {
                return this.CurrentBeingProcessedEvent == null && !this.OnTransition && this.queue.Count == 0;
            }
        }
    }
}
