namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class WorkFlowExecution
    {
        public static int Add(Model.WorkFlowExecution workinfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@DID", SqlDbType.Int), new SqlParameter("@UID", SqlDbType.Int), new SqlParameter("@step", SqlDbType.Int), new SqlParameter("@Remark", SqlDbType.VarChar, 255), new SqlParameter("@Result", SqlDbType.TinyInt) };
            pars[0].Value = workinfo.DID;
            pars[1].Value = workinfo.UID;
            pars[2].Value = workinfo.step;
            pars[3].Value = workinfo.Remark;
            pars[4].Value = workinfo.Result;
            return SqlHelper.ExecuteProcess("pro_WorkFlowExecution_Add", pars);
        }
    }
}

