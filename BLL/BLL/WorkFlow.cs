namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class WorkFlow
    {
        public static int WorkFlowAdd(Model.WorkFlow workinfo)
        {
            return DAL.WorkFlow.WorkFlowAdd(workinfo);
        }

        public static int WorkFlowDel(string ID)
        {
            return DAL.WorkFlow.WorkFlowDel(ID);
        }

        public static int WorkFlowUpdate(Model.WorkFlow workinfo)
        {
            return DAL.WorkFlow.WorkFlowUpdate(workinfo);
        }
    }
}

