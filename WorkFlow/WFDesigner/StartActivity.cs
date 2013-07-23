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
    public sealed class StartActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string FlowInstranceID = context.WorkflowInstanceId.ToString();
                if (String.IsNullOrEmpty(FlowInstranceID))
                    return;
                //BLL.Document.DocumentEndStep(FlowInstranceID);
            }
            catch(Exception e)
            {
                //执行出错
            }
        }
    }

}
