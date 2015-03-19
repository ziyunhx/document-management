using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.Metadata;
using System.Reflection;
using System.Activities.Presentation.Toolbox;
using ActivityLibrary;

namespace WFDesigner
{
   public  class toolBox
    {
       
        public static void loadSystemIcon()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            string str = System.Environment.CurrentDirectory + @"\Microsoft.VisualStudio.Activities.dll";
            Assembly sourceAssembly = Assembly.LoadFile(str);


            System.Resources.ResourceReader resourceReader = new System.Resources.ResourceReader(sourceAssembly.GetManifestResourceStream("Microsoft.VisualStudio.Activities.Resources.resources"));
            foreach (Type type in typeof(System.Activities.Activity).Assembly.GetTypes())
            {
                if (type.Namespace == "System.Activities.Statements")
                {
                    createImageToActivity(builder, resourceReader, type);
                }
            }
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        private static void createImageToActivity(AttributeTableBuilder builder, System.Resources.ResourceReader resourceReader, Type builtInActivityType)
        {
            System.Drawing.Bitmap bitmap = getImageFromResource(resourceReader, builtInActivityType.IsGenericType ? builtInActivityType.Name.Split('`')[0] : builtInActivityType.Name);
            if (bitmap != null)
            {
                Type tbaType = typeof(System.Drawing.ToolboxBitmapAttribute);
                Type imageType = typeof(System.Drawing.Image);
                ConstructorInfo constructor = tbaType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { imageType, imageType }, null);
                System.Drawing.ToolboxBitmapAttribute tba = constructor.Invoke(new object[] { bitmap, bitmap }) as System.Drawing.ToolboxBitmapAttribute;
                builder.AddCustomAttributes(builtInActivityType, tba);
            }
        }

        private static System.Drawing.Bitmap getImageFromResource(System.Resources.ResourceReader resourceReader, string bitmapName)
        {
            System.Collections.IDictionaryEnumerator dictEnum = resourceReader.GetEnumerator();
            System.Drawing.Bitmap bitmap = null;
            while (dictEnum.MoveNext())
            {
                if (String.Equals(dictEnum.Key, bitmapName))
                {
                    bitmap = dictEnum.Value as System.Drawing.Bitmap;
                    System.Drawing.Color pixel = bitmap.GetPixel(bitmap.Width - 1, 0);
                    bitmap.MakeTransparent(pixel);
                    break;
                }
            }
            return bitmap;
        }



        public static ToolboxCategoryItems loadToolbox()
        {
            loadSystemIcon();

            ToolboxCategoryItems toolboxCategoryItems = new ToolboxCategoryItems();

            //流程图
            ToolboxItemWrapper flowchar = new ToolboxItemWrapper(typeof(System.Activities.Statements.Flowchart), "Flowchart");
            ToolboxItemWrapper flowDecision = new ToolboxItemWrapper(typeof(System.Activities.Statements.FlowDecision), "FlowDecision");
            ToolboxItemWrapper flowSwitch = new ToolboxItemWrapper(typeof(System.Activities.Statements.FlowSwitch<string>), "FlowSwitch");

            ToolboxCategory wf4Flowchar = new System.Activities.Presentation.Toolbox.ToolboxCategory("流程图");

            wf4Flowchar.Add(flowchar);
            wf4Flowchar.Add(flowDecision);
            wf4Flowchar.Add(flowSwitch);

            toolboxCategoryItems.Add(wf4Flowchar);

            //状态机

            ToolboxItemWrapper stateMachineWithInitialStateFactory = new ToolboxItemWrapper(typeof(Machine.Design.ToolboxItems.StateMachineWithInitialStateFactory), "状态机流程");
            ToolboxItemWrapper state = new ToolboxItemWrapper(typeof(Machine.State), "节点");
            ToolboxCategory stateMachineActivity = new System.Activities.Presentation.Toolbox.ToolboxCategory("状态机");

            stateMachineActivity.Add(stateMachineWithInitialStateFactory);
            stateMachineActivity.Add(state);

            toolboxCategoryItems.Add(stateMachineActivity);




            //WF4.0 Activity
            ToolboxItemWrapper writeLine = new ToolboxItemWrapper(typeof(System.Activities.Statements.WriteLine), "WriteLine");
            ToolboxItemWrapper sequence = new ToolboxItemWrapper(typeof(System.Activities.Statements.Sequence), "Sequence");
            ToolboxItemWrapper Assign = new ToolboxItemWrapper(typeof(System.Activities.Statements.Assign), "Assign");
            ToolboxItemWrapper Delay = new ToolboxItemWrapper(typeof(System.Activities.Statements.Delay), "Delay");
            ToolboxItemWrapper If = new ToolboxItemWrapper(typeof(System.Activities.Statements.If), "If");
            ToolboxItemWrapper ForEach = new ToolboxItemWrapper(typeof(System.Activities.Statements.ForEach<string>), "ForEach");
            ToolboxItemWrapper Switch = new ToolboxItemWrapper(typeof(System.Activities.Statements.Switch<string>), "Switch");
            ToolboxItemWrapper While = new ToolboxItemWrapper(typeof(System.Activities.Statements.While), "While");
            ToolboxItemWrapper DoWhile = new ToolboxItemWrapper(typeof(System.Activities.Statements.DoWhile), "DoWhile");
            ToolboxItemWrapper Parallel = new ToolboxItemWrapper(typeof(System.Activities.Statements.Parallel), "Parallel");
            ToolboxItemWrapper Pick = new ToolboxItemWrapper(typeof(System.Activities.Statements.Pick), "Pick");
            ToolboxItemWrapper PickBranch = new ToolboxItemWrapper(typeof(System.Activities.Statements.PickBranch), "PickBranch");


            ToolboxCategory wf4Activity = new System.Activities.Presentation.Toolbox.ToolboxCategory("Activity");
            
            wf4Activity.Add(writeLine);
            wf4Activity.Add(sequence);
            wf4Activity.Add(Assign);
            wf4Activity.Add(Delay);
            wf4Activity.Add(If);
            wf4Activity.Add(ForEach);
            wf4Activity.Add(Switch);
            wf4Activity.Add(While);
            wf4Activity.Add(DoWhile);
            wf4Activity.Add(Parallel);
            wf4Activity.Add(Pick);
            wf4Activity.Add(PickBranch);

            toolboxCategoryItems.Add(wf4Activity);


            //文档活动
            //ToolboxItemWrapper StartActivity = new ToolboxItemWrapper(typeof(StartActivity), "开始活动");
            ToolboxItemWrapper DocActivity = new ToolboxItemWrapper(typeof(DocActivity), "文档审批");
            ToolboxItemWrapper EndActivity = new ToolboxItemWrapper(typeof(EndActivity), "结束活动");
            ToolboxCategory DocActivitys = new System.Activities.Presentation.Toolbox.ToolboxCategory("文档活动");
            //DocActivitys.Add(StartActivity);
            DocActivitys.Add(DocActivity);
            DocActivitys.Add(EndActivity);

            toolboxCategoryItems.Add(DocActivitys);

            return toolboxCategoryItems;

        }
    }
}
