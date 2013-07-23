using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Activities.Statements;
using System.Activities;

namespace Machine
{
    sealed class InternalTransition
    {
        Collection<TransitionData> transitionDataList;

        /// <summary>
        /// The index of this InternalTransition in internalTransitions list of its parent state.
        /// </summary>
        public int InternalTransitionIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Trigger object of this internal transition.
        /// </summary>
        public Activity Trigger
        {
            get;
            set;
        }

        /// <summary>
        /// TransitionDataList contains Tos, Conditions, Actions of different transitions which share the same trigger.
        /// </summary>
        public Collection<TransitionData> TransitionDataList
        {
            get
            {
                if (this.transitionDataList == null)
                {
                    this.transitionDataList = new Collection<TransitionData>();
                }
                return this.transitionDataList;
            }

        }

        /// <summary>
        /// IsUnconditional is used to denote whether this transition is unconditional.
        /// </summary>
        public bool IsUnconditional
        {
            get
            {
                return transitionDataList.Count == 1 && transitionDataList[0].Condition == null;
            }
        }
    }
}
