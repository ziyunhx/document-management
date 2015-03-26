using System.Collections.Generic;

namespace WorkFlow.Core
{
    public class WFLine
    {
        public WFLine()
        {
            connectorPoint = new List<WFPoint>();
        }

        public List<WFPoint> connectorPoint { set; get; }

        public string beginNodeID { set; get; }

        public string endNodeID { set; get; }

        public string text { set; get; }

    }

    public class WFPoint
    {
        public double x { set; get; }
        public double y { set; get; }
    }
}
