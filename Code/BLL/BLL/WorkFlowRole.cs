namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class WorkFlowRole
    {
        public static int WorkFlowRoleAdd(Model.WorkFlowRole workrole)
        {
            return DAL.WorkFlowRole.WorkFlowRoleAdd(workrole);
        }

        public static int WorkFlowRoleDel(string ID)
        {
            return DAL.WorkFlowRole.WorkFlowRoleDel(ID);
        }

        public static int WorkFlowRoleUpdate(Model.WorkFlowRole workrole)
        {
            return DAL.WorkFlowRole.WorkFlowRoleUpdate(workrole);
        }
    }
}

