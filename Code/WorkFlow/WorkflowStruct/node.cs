using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkflowStruct
{
    public class node
    {
        public node()
        {
            ShapeSize = new nodeSize();
        }

        public nodeSize ShapeSize { set; get; }

        public string DisplayName { set; get; }

        public string Content { set; get; }

        public string id { set; get; }

    }


    public class switchNode : node
    {
        public switchNode()
        {
            switchCaseList = new List<string>();
        }

        public List<string > switchCaseList { set; get; }

        public string switchDefault { set; get; }
    }

    public class nodeSize
    {
        public double width { set; get; }

        public double height { set; get; }

        public double x { set; get; }

        public double y { set; get; }
    }
}
