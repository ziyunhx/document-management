namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class WorkFlowExecution
    {
        public static int Add(Model.WorkFlowExecution workinfo)
        {
            return DAL.WorkFlowExecution.Add(workinfo);
        }
    }
}

