using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Activities;

namespace Machine
{
  
    public sealed class State 
    {

        InternalState internalState;
        Collection<State> childStates;
        Collection<Transition> transitions;

 
        public  string DisplayName
        {
            get;
            set;
        }

  
        [DefaultValue(null)]
        public Activity Entry
        {
            get;
            set;
        }

   
        [DependsOn("Entry")]
        [DefaultValue(null)]
        public Activity Exit
        {
            get;
            set;
        }

 
        [DependsOn("Exit")]
        public Collection<State> States
        {
            get
            {
                if (this.childStates == null)
                {
                    this.childStates = new ValidatingCollection<State>
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
                return this.childStates;

            }
        }

   
        [DependsOn("ChildStates")]
        public Collection<Transition> Transitions
        {
            get
            {
                if (this.transitions == null)
                {
                    this.transitions = new ValidatingCollection<Transition>
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
                return this.transitions;
            }
        }

        /// <summary>
        /// IsFinal represents whether the State is a final State.
        /// </summary>
        [DefaultValue(false)]
        public bool IsFinal
        {
            get;
            set;
        }

        /// <summary>
        /// Internal activity representation of state.
        /// </summary>
        internal InternalState InternalState
        {
            get
            {
                if (this.internalState == null)
                {
                    this.internalState = new InternalState(this);
                }
                return this.internalState;
            }
        }

        /// <summary>
        /// StateId is unique within a StateMachine.
        /// </summary>
        internal string StateId
        {
            get;
            set;
        }

        /// <summary>
        /// HasInheritedTransition denote whether ancestors of state have any transitions.
        /// </summary>
        internal bool HasInheritedTransition
        {
            get;
            set;
        }

        /// <summary>
        /// ParentState has a reference to parent state. 
        /// If this state is a root level state, the parent of it is null.
        /// </summary>
        internal State ParentState
        {
            get;
            set;
        }

        /// <summary>
        /// Reached denotes whether state can be reached via transitions.
        /// </summary>
        internal bool Reachable
        {
            get;
            set;
        }

        /// <summary>
        /// PassNumber is used to detect re-visiting when traversing states in StateMachine. 
        /// </summary>
        internal uint PassNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Clear internal state. 
        /// </summary>
        internal void ClearInternalState()
        {
            this.internalState = null;
        }
    }
}
