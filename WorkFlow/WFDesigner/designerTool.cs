using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation;
using System.Activities.Presentation.View;

namespace WFDesigner
{
   public  class designerTool
    {
       public static IEnumerable<ModelItem> getSelectActivityList(WorkflowDesigner designer)
       {
           foreach (var v in designer.Context.Items)
           {
               Selection selection = v as Selection;
               if (selection != null)
               {
                   return selection.SelectedObjects;
               }
           }
            return null;
       } 

       public static string getXamlFilePath(WorkflowDesigner designer)
       {
           System.Activities.Presentation.WorkflowFileItem fileItem = designer.Context.Items.GetValue(typeof(System.Activities.Presentation.WorkflowFileItem)) as System.Activities.Presentation.WorkflowFileItem;
           return fileItem.LoadedFile;
       }

    }
}
