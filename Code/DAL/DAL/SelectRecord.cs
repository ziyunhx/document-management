namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class SelectRecord
    {
        public static DataSet SelectRecordData(Model.SelectRecord selectRecord)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@tablename", SqlDbType.VarChar, 100), new SqlParameter("@record", SqlDbType.VarChar, 200), new SqlParameter("@columnlist", SqlDbType.VarChar, 300), new SqlParameter("@condition", SqlDbType.VarChar, 0x1f40) };
            pars[0].Value = selectRecord.Stablename;
            pars[1].Value = selectRecord.Irecord;
            pars[2].Value = selectRecord.Scolumnlist;
            pars[3].Value = selectRecord.Scondition;
            return SqlHelper.GetAllInfo(pars, "SelectRecord");
        }
    }
}

