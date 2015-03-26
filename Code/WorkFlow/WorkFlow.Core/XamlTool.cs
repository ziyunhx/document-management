using System;
using System.Activities;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using System.Xml;

namespace WorkFlow.Core
{
    public static class XamlTool
    {
        public static Activity getActivityByString(string xaml)
        {
            System.IO.StringReader stringReader = new System.IO.StringReader(xaml);

            Activity activity = System.Activities.XamlIntegration.ActivityXamlServices.Load(stringReader);

            WorkflowInspectionServices.CacheMetadata(activity);

            return activity;
        }//end 

        public static string getStringByXamlFile(string filePathName)
        {
            string xamlString = "";
            xamlString = getStringByFile(filePathName);
            return xamlString;
        }//end 

        public static DynamicActivity getDynamicActivityByString(string xaml)
        {
            DynamicActivity dynamicActivity = getActivityByString(xaml) as DynamicActivity;

            if (dynamicActivity != null)
            {
                return dynamicActivity;
            }
            else
            {
                return null;
            }
        }//end 

        public static Activity getRuntimeActivityByDynamicActivity(DynamicActivity dynamicActivity)
        {
            IEnumerator<Activity> list = WorkflowInspectionServices.GetActivities(dynamicActivity).GetEnumerator();

            list.MoveNext();

            Activity runtimeActivity = list.Current;

            return runtimeActivity;
        }

        public static bool isDynamicActivity(string xaml)
        {
            object obj = getDynamicActivityByString(xaml);
            if (obj != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void writeActivityToFile(Activity activity, string fileName)
        {
            XamlServices.Save(fileName, activity);
        }

        public static string getStringByFile(string filePathName)
        {
            string valueString = "";

            using (System.IO.FileStream fileStream = new System.IO.FileStream(filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream))
                {
                    valueString = streamReader.ReadToEnd();
                }
            }
            return valueString;
        }


        public static string removeViewState(ActivityBuilder activityBuilder)
        {
            string xamlString = "";

            XmlWriterSettings writerSettings = new XmlWriterSettings { Indent = true };

            StringBuilder stringBuilder = new StringBuilder();


            XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, writerSettings);

            XamlXmlWriter xamlXmlWriter = new XamlXmlWriter(xmlWriter, new XamlSchemaContext());

            XamlWriter xamlWriter = ActivityXamlServices.CreateBuilderWriter(xamlXmlWriter);

            viewStateXamlWriter wfViewStateControl = new viewStateXamlWriter(xamlWriter);

            XamlServices.Save(wfViewStateControl, activityBuilder);

            xamlString = stringBuilder.ToString();

            return xamlString;

        }

        public static Dictionary<string, object> getNodeViewState(object activity)
        {
            Dictionary<string, object> list = WorkflowViewStateService.GetViewState(activity);
            foreach (var v in list)
            {
                System.Console.WriteLine(v);
            }

            return list;
        }


        public static FlowcharStruct getFlowcharStruct(string xaml)
        {
            char[] sp = { ',' };
            char[] sl = { ' ' };
            //(1)
            FlowcharStruct flowcharStruct = new FlowcharStruct();

            //(2)
            DynamicActivity dynamicActivity = XamlTool.getDynamicActivityByString(xaml);
            Activity activity = XamlTool.getRuntimeActivityByDynamicActivity(dynamicActivity);
            Flowchart flowchar = activity as Flowchart;

            if (flowchar == null)
            {
                return null;
            }

            //(3)=====================================

            //(3.1)----------------------------------------------------------------------------------
            flowcharStruct.beginNode.DisplayName = "开始";
            flowcharStruct.beginNode.id = "begin";

            //(3.1.1)
            if (WorkflowViewStateService.GetViewState(flowchar)["ShapeLocation"] != null)
            {
                string ShapeLocation = WorkflowViewStateService.GetViewState(flowchar)["ShapeLocation"].ToString();
                flowcharStruct.beginNode.ShapeSize.x = double.Parse(ShapeLocation.Split(sp)[0]);
                flowcharStruct.beginNode.ShapeSize.y = double.Parse(ShapeLocation.Split(sp)[1]);
            }

            //(3.1.2)
            if (WorkflowViewStateService.GetViewState(flowchar)["ShapeSize"] != null)
            {
                string ShapeSize = WorkflowViewStateService.GetViewState(flowchar)["ShapeSize"].ToString();
                flowcharStruct.beginNode.ShapeSize.width = double.Parse(ShapeSize.Split(sp)[0]);
                flowcharStruct.beginNode.ShapeSize.height = double.Parse(ShapeSize.Split(sp)[1]);
            }

            //(3.1.3)
            if (WorkflowViewStateService.GetViewState(flowchar)["ConnectorLocation"] != null)
            {
                string ConnectorLocation = WorkflowViewStateService.GetViewState(flowchar)["ConnectorLocation"].ToString();
                string[] points = ConnectorLocation.Split(sl);
                WFLine oneline = new WFLine();
                oneline.beginNodeID = flowchar.Id;
                oneline.text = flowchar.DisplayName;
                foreach (string item in points)
                {
                    double x = double.Parse(item.Split(sp)[0]);
                    double y = double.Parse(item.Split(sp)[1]);
                    oneline.connectorPoint.Add(new WFPoint() { x = x, y = y });
                }
                flowcharStruct.lineList.Add(oneline);
            }

            //(3.2)--------------------------------------------------------------------------------
            foreach (FlowNode flowNode in flowchar.Nodes)
            {
                FlowStep flowStep = flowNode as FlowStep;
                if (flowStep != null)
                {
                    WFNode node = new WFNode();

                    node.DisplayName = flowStep.Action.DisplayName;
                    node.id = flowStep.Action.Id;

                    //(3.2.1)
                    if (WorkflowViewStateService.GetViewState(flowStep)["ShapeLocation"] != null)
                    {
                        string ShapeLocation = WorkflowViewStateService.GetViewState(flowStep)["ShapeLocation"].ToString();
                        node.ShapeSize.x = double.Parse(ShapeLocation.Split(sp)[0]);
                        node.ShapeSize.y = double.Parse(ShapeLocation.Split(sp)[1]);
                    }

                    //(3.2.2)
                    if (WorkflowViewStateService.GetViewState(flowStep)["ShapeSize"] != null)
                    {
                        string ShapeSize = WorkflowViewStateService.GetViewState(flowStep)["ShapeSize"].ToString();
                        node.ShapeSize.width = double.Parse(ShapeSize.Split(sp)[0]);
                        node.ShapeSize.height = double.Parse(ShapeSize.Split(sp)[1]);
                    }

                    //(3.2.3)
                    if (WorkflowViewStateService.GetViewState(flowStep).Count(p => p.Key == "ConnectorLocation") == 1)
                    {
                        string ConnectorLocation = WorkflowViewStateService.GetViewState(flowStep)["ConnectorLocation"].ToString();
                        string[] points = ConnectorLocation.Split(sl);
                        WFLine line = new WFLine();
                        line.beginNodeID = flowStep.Action.Id;
                        line.text = flowStep.Action.DisplayName;
                        foreach (string item in points)
                        {
                            double x = double.Parse(item.Split(sp)[0]);
                            double y = double.Parse(item.Split(sp)[1]);
                            line.connectorPoint.Add(new WFPoint() { x = x, y = y });
                        }
                        flowcharStruct.lineList.Add(line);
                    }

                    flowcharStruct.nodeList.Add(node);
                }
            }

            //(3.3)-------------------------------------------------------------
            foreach (FlowNode flowNode in flowchar.Nodes)
            {

                FlowSwitch<string> flowSwitch = flowNode as FlowSwitch<string>;

                if (flowSwitch != null)
                {
                    WFNode node = new WFNode();

                    node.DisplayName = flowSwitch.Expression.DisplayName;
                    node.id = flowSwitch.Expression.Id;

                    //(3.3.1)
                    if (WorkflowViewStateService.GetViewState(flowSwitch)["ShapeLocation"] != null)
                    {
                        string ShapeLocation = WorkflowViewStateService.GetViewState(flowSwitch)["ShapeLocation"].ToString();
                        node.ShapeSize.x = double.Parse(ShapeLocation.Split(sp)[0]);
                        node.ShapeSize.y = double.Parse(ShapeLocation.Split(sp)[1]);
                    }

                    //(3.3.2)
                    if (WorkflowViewStateService.GetViewState(flowSwitch)["ShapeSize"] != null)
                    {
                        string ShapeSize = WorkflowViewStateService.GetViewState(flowSwitch)["ShapeSize"].ToString();
                        node.ShapeSize.width = double.Parse(ShapeSize.Split(sp)[0]);
                        node.ShapeSize.height = double.Parse(ShapeSize.Split(sp)[1]);
                    }

                    //(3.3.3)

                    if (WorkflowViewStateService.GetViewState(flowSwitch).Count(p => p.Key == "Default") == 1)
                    {
                        string ConnectorLocation = WorkflowViewStateService.GetViewState(flowSwitch)["Default"].ToString();
                        string[] points = ConnectorLocation.Split(sl);
                        WFLine line = new WFLine();
                        line.beginNodeID = flowSwitch.Expression.Id;
                        line.text = flowSwitch.Expression.DisplayName;
                        foreach (string item in points)
                        {
                            double x = double.Parse(item.Split(sp)[0]);
                            double y = double.Parse(item.Split(sp)[1]);
                            line.connectorPoint.Add(new WFPoint() { x = x, y = y });
                        }
                        flowcharStruct.lineList.Add(line);
                    }

                    //(3.3.4)
                    foreach (var v in flowSwitch.Cases)
                    {
                        FlowNode next = v.Value;
                        Console.WriteLine(v.Key);
                        string caseValue = v.Key + "Connector";
                        if (WorkflowViewStateService.GetViewState(flowSwitch).Count(p => p.Key == caseValue) == 1)
                        {
                            string ConnectorLocation = WorkflowViewStateService.GetViewState(flowSwitch)[caseValue].ToString();
                            string[] points = ConnectorLocation.Split(sl);
                            WFLine line = new WFLine();
                            line.beginNodeID = flowSwitch.Expression.Id;
                            line.text = flowSwitch.Expression.DisplayName;
                            foreach (string item in points)
                            {
                                double x = double.Parse(item.Split(sp)[0]);
                                double y = double.Parse(item.Split(sp)[1]);
                                line.connectorPoint.Add(new WFPoint() { x = x, y = y });
                            }
                            flowcharStruct.lineList.Add(line);
                        }
                    }
                    flowcharStruct.nodeList.Add(node);
                }
            }

            return flowcharStruct;

        }//end

    }

    public class viewStateXamlWriter : XamlWriter
    {
        public viewStateXamlWriter(XamlWriter innerWriter)
        {
            this.InnerWriter = innerWriter;
            this.MemberStack = new Stack<XamlMember>();
        }

        XamlWriter InnerWriter { get; set; }
        Stack<XamlMember> MemberStack { get; set; }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (InnerWriter != null)
                {
                    ((IDisposable)InnerWriter).Dispose();
                    InnerWriter = null;
                }

                MemberStack.Clear();
            }

            base.Dispose(disposing);
        }

        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return InnerWriter.SchemaContext;
            }
        }

        public override void WriteEndMember()
        {
            XamlMember xamlMember = MemberStack.Pop();
            if (m_attachedPropertyDepth > 0)
            {
                if (IsDesignerAttachedProperty(xamlMember))
                {
                    m_attachedPropertyDepth--;
                }
                return;
            }

            InnerWriter.WriteEndMember();
        }

        public override void WriteEndObject()
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteEndObject();
        }

        public override void WriteGetObject()
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteGetObject();
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteNamespace(namespaceDeclaration);
        }

        public override void WriteStartMember(XamlMember xamlMember)
        {
            MemberStack.Push(xamlMember);
            if (IsDesignerAttachedProperty(xamlMember))
            {
                m_attachedPropertyDepth++;
            }

            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteStartMember(xamlMember);
        }

        public override void WriteStartObject(XamlType type)
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteStartObject(type);
        }

        public override void WriteValue(Object value)
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteValue(value);
        }

        static Boolean IsDesignerAttachedProperty(XamlMember xamlMember)
        {
            return xamlMember.IsAttachable &&
                   xamlMember.PreferredXamlNamespace.Equals(c_sapNamespaceURI, StringComparison.OrdinalIgnoreCase);
        }

        private Int32 m_attachedPropertyDepth = 0;
        const String c_sapNamespaceURI = "http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation";
    }//end class



    public static class activityStructTool
    {
        static void printActivityTree(Activity activity, string tag)
        {
            Console.WriteLine("{0} DisplayName:{1},type:{2}", tag, activity.DisplayName, activity.GetType());

            IEnumerator<Activity> list = WorkflowInspectionServices.GetActivities(activity).GetEnumerator();

            while (list.MoveNext())
            {
                printActivityTree(list.Current, "  " + tag);
            }
        }

        public static void printActivityTree(Activity activity)
        {
            printActivityTree(activity, "|--");
        }

        static activityStruct tag;
        public static void printActivityTreeA(Activity activity)
        {
            if (tag == null)
            {
                tag = new activityStruct();
                tag.parent = null;
                tag.currentActivity = activity;
                tag.displayName = activity.DisplayName;

            }

            IEnumerator<Activity> list = WorkflowInspectionServices.GetActivities(activity).GetEnumerator();

            while (list.MoveNext())
            {
                activityStruct temp = new activityStruct();

                temp.parent = tag;
                temp.currentActivity = list.Current;
                temp.displayName = list.Current.DisplayName;
                printActivityTreeA(list.Current);
            }
        }

        public class activityStruct
        {
            public activityStruct parent { set; get; }

            public List<activityStruct> nodes { set; get; }

            public string displayName { set; get; }

            public Activity currentActivity { set; get; }

            public activityStruct()
            {
                nodes = new List<activityStruct>();
            }
        }
    }
}