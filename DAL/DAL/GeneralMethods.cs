using DBAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class GeneralMethods
    {
        public static int GeneralDelDB(string tablename, string where)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@TABLE_NAME", SqlDbType.VarChar, 50), new SqlParameter("@FIELD_WHERE", SqlDbType.VarChar, 200) };
            pars[0].Value = tablename;
            pars[1].Value = where;
            return SqlHelper.ExecuteProcess("PRO_DELETE_DB", pars);
        }

        public static int GeneralInsertDB(string tablename, string filename, string filevale)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@TABLE_NAME", SqlDbType.VarChar, 50), new SqlParameter("@FIELD_NAME", SqlDbType.VarChar, 0x3e8), new SqlParameter("@FIELD_VALUE", SqlDbType.VarChar, 0x1388) };
            pars[0].Value = tablename;
            pars[1].Value = filename;
            pars[2].Value = filevale;
            return SqlHelper.ExecuteProcess("PRO_INSERT_DB", pars);
        }

        public static int GeneralUPdateDB(string tablename, string filename, string filevalue, string where)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@TABLE_NAME", SqlDbType.VarChar, 50), new SqlParameter("@FIELD_NAME", SqlDbType.VarChar, 0x3e8), new SqlParameter("@FIELD_VALUE", SqlDbType.VarChar, 0x5dc), new SqlParameter("@FIELD_WHERE", SqlDbType.VarChar, 200) };
            pars[0].Value = tablename;
            pars[1].Value = filename;
            pars[2].Value = filevalue;
            pars[3].Value = where;
            return SqlHelper.ExecuteProcess("PRO_UPDATE_DB", pars);
        }
    }
}

