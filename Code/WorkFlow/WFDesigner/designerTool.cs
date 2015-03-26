using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections.Generic;

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
           WorkflowFileItem fileItem = designer.Context.Items.GetValue(typeof(WorkflowFileItem)) as WorkflowFileItem;
           return fileItem.LoadedFile;
       }
    }
}
