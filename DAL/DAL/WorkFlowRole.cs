namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class WorkFlowRole
    {
        public static int WorkFlowRoleAdd(Model.WorkFlowRole workrole)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@OID", SqlDbType.VarChar, 255), new SqlParameter("@WID", SqlDbType.Int ), new SqlParameter("@WSTEP", SqlDbType.Int), new SqlParameter("@name", SqlDbType.VarChar, 50), new SqlParameter("@value", SqlDbType.VarChar, 50), new SqlParameter("@State", SqlDbType.TinyInt) };
            pars[0].Value = workrole.OID;
            pars[1].Value = workrole.WID;
            pars[2].Value = workrole.WSTEP;
            pars[3].Value = workrole.name;
            pars[4].Value = workrole.value;
            pars[5].Value = workrole.State;
            return SqlHelper.ExecuteProcess("pro_WorkFlowRole_Add", pars);
        }

        public static int WorkFlowRoleDel(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_WorkFlowRole_Del", pars);
        }

        public static int WorkFlowRoleUpdate(Model.WorkFlowRole workrole)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@OID", SqlDbType.VarChar, 255), new SqlParameter("@WID", SqlDbType.Int), new SqlParameter("@WSTEP", SqlDbType.Int), new SqlParameter("@name", SqlDbType.VarChar, 50), new SqlParameter("@value", SqlDbType.VarChar, 50), new SqlParameter("@State", SqlDbType.TinyInt) };
            pars[0].Value = workrole.ID;
            pars[1].Value = workrole.OID;
            pars[2].Value = workrole.WID;
            pars[3].Value = workrole.WSTEP;
            pars[4].Value = workrole.name;
            pars[5].Value = workrole.value;
            pars[6].Value = workrole.State;
            return SqlHelper.ExecuteProcess("pro_WorkFlowRole_Update", pars);
        }
    }
}

