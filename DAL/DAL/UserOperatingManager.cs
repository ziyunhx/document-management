namespace DAL
{
    using DBAccess;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class UserOperatingManager
    {
        public static int InputUserOperating(string uid, string actionType, string Descriptions)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@uid", SqlDbType.Int), new SqlParameter("@actionType", SqlDbType.VarChar, 20), new SqlParameter("@Descriptions", SqlDbType.VarChar, 100) };
            pars[0].Value = uid;
            pars[1].Value = actionType;
            pars[2].Value = Descriptions;
            return SqlHelper.ExecuteProcess("pro_InputUserOperating", pars);
        }
    }
}

