using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using Commons;
using BLL;

namespace WFDesigner
{
    public sealed class EndActivity :CodeActivity
    {
        public System.Activities.InArgument<string> s
        { set; get; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string FlowInstranceID = context.WorkflowInstanceId.ToString();
                string result = s.Get(context);
                if (String.IsNullOrEmpty(FlowInstranceID))
                    return;
                if (result == "true")
                {
                    BLL.Document.DocumentEndStep(FlowInstranceID, "1");
                }
                else
                {
                    BLL.Document.DocumentEndStep(FlowInstranceID, "2");
                }
            }
            catch(Exception e)
            {
                //执行出错
            }
        }
    }

}
