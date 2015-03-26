using System.Collections.Generic;

namespace WorkFlow.Core
{
    public class FlowcharStruct
    {
       public FlowcharStruct()
       {
           nodeList = new List<WFNode>();
           lineList = new List<WFLine>();
           beginNode = new WFNode();
           endNode = new WFNode();
       }

       public WFNode beginNode { set; get; }
     
       public WFNode endNode { set; get; }

       public List<WFNode> nodeList { set; get; }

       public List<WFLine> lineList { set; get; }
    }
}
