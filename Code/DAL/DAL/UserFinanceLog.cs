namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class UserFinanceLog
    {
        public static DataSet get_top_Sum_Date(string start, string end, string type)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@startT", SqlDbType.DateTime), new SqlParameter("@endT", SqlDbType.DateTime), new SqlParameter("@type", SqlDbType.Int) };
            pars[0].Value = start;
            pars[1].Value = end;
            pars[2].Value = type;
            return SqlHelper.GetAllInfo(pars, "get_top_Sum_Date");
        }

        public static DataTable getUserMonthStatistics(string UID, string SqlWhere)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@uid", SqlDbType.Int), new SqlParameter("@SqlWhere", SqlDbType.VarChar, 300) };
            pars[0].Value = UID;
            pars[1].Value = SqlWhere;
            return SqlHelper.GetAllInfo(pars, "pro_GetMonthStatistics").Tables[0];
        }

        public static int UpdateStrField(string table, string filed, string filedvalue, string contion)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@table", SqlDbType.VarChar, 0x3e8), new SqlParameter("@filed", SqlDbType.VarChar, 0x3e8), new SqlParameter("@filedvalue", SqlDbType.VarChar, 0x7d0), new SqlParameter("@contion", SqlDbType.VarChar, 0x3e8) };
            pars[0].Value = table;
            pars[1].Value = filed;
            pars[2].Value = filedvalue;
            pars[3].Value = contion;
            return SqlHelper.ExecuteProcess("UpdateStrField", pars);
        }

        public static int UserFinanceLogAdd(Model.UserFinanceLog userfinanceLog)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@UID", SqlDbType.Int), new SqlParameter("@FinanceType", SqlDbType.Int), new SqlParameter("@Amount", SqlDbType.Money), new SqlParameter("@FinanceInfo", SqlDbType.VarChar, 500), new SqlParameter("@IsState", SqlDbType.Int) };
            pars[0].Value = userfinanceLog.UID;
            pars[1].Value = userfinanceLog.FinanceType;
            pars[2].Value = userfinanceLog.Amount;
            pars[3].Value = userfinanceLog.FinanceInfo;
            pars[4].Value = userfinanceLog.IsState;
            return SqlHelper.ExecuteProcess("pro_UserFinanceLog_Add", pars);
        }

        public static int UserFinanceLogDelete(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_UserFinanceLog_Del", pars);
        }

        public static int UserFinanceLogUpdateForIsState(string ID, string IsState)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@IsState", SqlDbType.Int) };
            pars[0].Value = ID;
            pars[1].Value = IsState;
            return SqlHelper.ExecuteProcess("pro_UserFianceLog_IsState_Update", pars);
        }

        public static int UserWithdrawalsCancel(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@id", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_withdrawals_Cancel", pars);
        }

        public static int YZM_ChongZhi_Success(int id)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = id;
            return SqlHelper.ExecuteProcess("YZM_ChongZhi_Success", pars);
        }

        public static int YZM_TiXian_ShenQing(int uid, decimal amount, string financeInfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@uid", SqlDbType.Int), new SqlParameter("@amount", SqlDbType.Decimal, 9), new SqlParameter("@financeInfo", SqlDbType.VarChar, 100) };
            pars[0].Value = uid;
            pars[1].Value = amount;
            pars[2].Value = financeInfo;
            return SqlHelper.ExecuteProcess("YZM_TiXian_ShenQing", pars);
        }

        public static int YZM_TiXian_Success(int id)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = id;
            return SqlHelper.ExecuteProcess("YZM_TiXian_Success", pars);
        }

        public static int YZM_TuiKuan_Success(string userID, decimal amount, string financeInfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@userID", SqlDbType.VarChar, 50), new SqlParameter("@amount", SqlDbType.Decimal, 9), new SqlParameter("@financeInfo", SqlDbType.VarChar, 100) };
            pars[0].Value = userID;
            pars[1].Value = amount;
            pars[2].Value = financeInfo;
            return SqlHelper.ExecuteProcess("YZM_TuiKuan_Success", pars);
        }
    }
}

