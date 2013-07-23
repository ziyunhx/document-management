using System;
using System.Data;
using System.Data.SqlClient;
using DBAccess;

namespace DAL
{



    public class RoleList
    {
        public static int DelMenuRIGHT(string roleid)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@roleid", SqlDbType.Int) };
            pars[0].Value = roleid;
            return SqlHelper.ExecuteProcess("pro_DeleteMenuRIGHT", pars);
        }

        public static int setRole(string name, string isstate)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@name", SqlDbType.VarChar, 30), new SqlParameter("@isState", SqlDbType.Int) };
            pars[0].Value = name;
            pars[1].Value = isstate;
            return SqlHelper.ExecuteProcess("pro_insertRole", pars);
        }

        public static int setMenuRIGHT(string flowid, string roleid)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@flowid", SqlDbType.Int), new SqlParameter("@roleid", SqlDbType.Int) };
            pars[0].Value = flowid;
            pars[1].Value = roleid;
            return SqlHelper.ExecuteProcess("pro_insertMenuRIGHT", pars);
        }

        public static int UpdateRole(string id, string name, string isstate)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@id", SqlDbType.Int), new SqlParameter("@name", SqlDbType.VarChar, 30), new SqlParameter("@isState", SqlDbType.Int) };
            pars[0].Value = id;
            pars[1].Value = name;
            pars[2].Value = isstate;
            return SqlHelper.ExecuteProcess("pro_UpdateRole", pars);
        }
    }
}

