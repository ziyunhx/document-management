namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class Organizational
    {
        public static int OrganAdd(Model.Organizational Organ)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@Name", SqlDbType.VarChar, 50), new SqlParameter("@PID", SqlDbType.VarChar, 6) };
            pars[0].Value = Organ.Name;
            pars[1].Value = Organ.PID;
            return SqlHelper.ExecuteProcess("pro_Organizational_Add", pars);
        }

        public static int OrganDelete(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_Organizational_Del", pars);
        }

        public static int OrganUpdate(Model.Organizational Organ)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@Name", SqlDbType.VarChar, 30), new SqlParameter("@PID", SqlDbType.VarChar, 6) };
            pars[0].Value = Organ.ID;
            pars[1].Value = Organ.Name;
            pars[2].Value = Organ.PID;
            return SqlHelper.ExecuteProcess("pro_Organizational_Update", pars);
        }
    }
}

