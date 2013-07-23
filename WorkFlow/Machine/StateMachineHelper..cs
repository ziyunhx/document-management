using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine
{
    using System.Collections.ObjectModel;
    using System.Activities;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Helper methods which are used by both StateMachine and State.
    /// </summary>
    static class StateMachineHelper
    {
        /// <summary>
        /// Create internal states
        /// </summary>
        public static void ProcessChildStates(NativeActivityMetadata metadata, Collection<State> childStates,
            Collection<InternalState> internalStates, Collection<ActivityFunc<string, StateMachineEventManager, string>> internalStateFuncs)
        {
            foreach (State state in childStates)
            {
                InternalState internalState = state.InternalState;
                internalStates.Add(internalState);
                DelegateInArgument<string> toStateId = new DelegateInArgument<string>();
                DelegateInArgument<StateMachineEventManager> eventManager = new DelegateInArgument<StateMachineEventManager>();
                internalState.ToState = toStateId;
                internalState.EventManager = eventManager;
                ActivityFunc<string, StateMachineEventManager, string> activityFunc = new ActivityFunc<string, StateMachineEventManager, string>
                {
                    Argument1 = toStateId,
                    Argument2 = eventManager,
                    Handler = internalState,
                };
                if (state.Reachable)
                {
                    //If this state is not reached, we should not add it as child because it's even not well validated.
                    metadata.AddDelegate(activityFunc);
                }
                internalStateFuncs.Add(activityFunc);
            }
        }

        #region Helpers on State Id
        /// <summary>
        /// Given current stateId and descendant Id, this method returns Id of direct child state of current state.
        /// This direct child state is either the state which descendantId represents or one of ancestor states of it.
        /// </summary>
        public static int GetChildStateIndex(string stateId, string descendantId)
        {
            Debug.Assert(!string.IsNullOrEmpty(descendantId), "descendantId should not be null or empty.");
            Debug.Assert(!string.IsNullOrEmpty(stateId), "stateId should not be null or empty.");
            string[] child = descendantId.Split('_');
            string[] parent = stateId.Split('_');
            Debug.Assert(parent.Length < child.Length, "stateId should not be null or empty.");
            return int.Parse(child[parent.Length], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method is used to see whether state1 is one of ancestors of state2.
        /// </summary>
        public static bool IsAncestor(string state1Id, string state2Id)
        {
            if (string.IsNullOrEmpty(state2Id)) return false;
            return state2Id.StartsWith(state1Id, StringComparison.Ordinal) && state2Id != state1Id;
        }
        #endregion
    }
}
