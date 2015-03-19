//------------------------------------------------------------

//------------------------------------------------------------

namespace Machine.Design.ToolboxItems
{
    using System.Activities;
    using System.Activities.Presentation;
    using System.Windows;
    


    public sealed class StateMachineWithInitialStateFactory : IActivityTemplateFactory
    {

        public Activity Create(DependencyObject target)
        {

            State state = new State()
            {
                DisplayName = "第一个业务节点"
            };
            return new StateMachine()
            {
                States = {state,new State(){ IsFinal=true}},
                InitialState = state
               
            };
        }
    }
}
