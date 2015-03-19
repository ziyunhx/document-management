namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class WorkFlow
    {
        public static int WorkFlowAdd(Model.WorkFlow workinfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@Name", SqlDbType.VarChar, 50), new SqlParameter("@URL", SqlDbType.VarChar, 100), new SqlParameter("@Remark", SqlDbType.VarChar, 255), new SqlParameter("@State", SqlDbType.Int) };
            pars[0].Value = workinfo.Name;
            pars[1].Value = workinfo.URL;
            pars[2].Value = workinfo.Remark;
            pars[3].Value = workinfo.State;
            return SqlHelper.ExecuteProcess("pro_WorkFlow_Add", pars);
        }

        public static int WorkFlowDel(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_WorkFlow_Del", pars);
        }

        public static int WorkFlowUpdate(Model.WorkFlow workinfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@Name", SqlDbType.VarChar, 50), new SqlParameter("@URL", SqlDbType.VarChar, 100), new SqlParameter("@Remark", SqlDbType.VarChar, 255), new SqlParameter("@State", SqlDbType.Int) };
            pars[0].Value = workinfo.ID;
            pars[1].Value = workinfo.Name;
            pars[2].Value = workinfo.URL;
            pars[3].Value = workinfo.Remark;
            pars[4].Value = workinfo.State;
            return SqlHelper.ExecuteProcess("pro_WorkFlow_Update", pars);
        }
    }
}

