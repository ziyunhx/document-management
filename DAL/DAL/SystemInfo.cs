using System;
using System.Data;
using System.Data.SqlClient;
using DBAccess;

namespace DAL
{
    public class SystemInfo
    {
        public static int UpdateInfo(string id, string value)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@id", SqlDbType.Int), new SqlParameter("@value", SqlDbType.VarChar, 1024) };
            pars[0].Value = id;
            pars[1].Value = value;
            return SqlHelper.ExecuteProcess("pro_UpdateInfo", pars);
        }
    }
}

