using System;
using System.Activities;

namespace ActivityLibrary
{
    public sealed class EndActivity :CodeActivity
    {
        public InArgument<string> s
        { set; get; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Guid FlowInstranceID = context.WorkflowInstanceId;
                DebugStatus debug = new DebugStatus();
                if (debug.status == 0)
                {
                    string result = s.Get(context);

                    if (result == "true")
                    {
                        //TODO Change the step.
                        //BLL.Document.DocumentEndStep(FlowInstranceID, "1");
                    }
                    else
                    {
                        //BLL.Document.DocumentEndStep(FlowInstranceID, "2");
                    }
                }
            }
            catch//(Exception e)
            {
                //执行出错
            }
        }
    }
}
