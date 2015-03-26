using System.Collections.Generic;

namespace WorkFlow.Core
{
    public class WFNode
    {
        public WFNode()
        {
            ShapeSize = new WFNodeSize();
        }

        public WFNodeSize ShapeSize { set; get; }

        public string DisplayName { set; get; }

        public string Content { set; get; }

        public string id { set; get; }

    }


    public class SwitchNode : WFNode
    {
        public SwitchNode()
        {
            switchCaseList = new List<string>();
        }

        public List<string > switchCaseList { set; get; }

        public string switchDefault { set; get; }
    }

    public class WFNodeSize
    {
        public double width { set; get; }

        public double height { set; get; }

        public double x { set; get; }

        public double y { set; get; }
    }
}
