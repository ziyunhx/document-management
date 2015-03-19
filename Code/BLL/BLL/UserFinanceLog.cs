namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class UserFinanceLog
    {
        public static DataSet get_top_Sum_Date(string start, string end, string type)
        {
            if (start == "")
            {
                start = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-01 00:00:00";
            }
            if (end == "")
            {
                end = DateTime.Now.ToShortDateString() + " 23:59:59";
            }
            else
            {
                end = end + " 23:59:59";
            }
            if ((type == "") || (type == "0"))
            {
                type = "2";
            }
            return DAL.UserFinanceLog.get_top_Sum_Date(start, end, type);
        }

        public static DataTable getUserMonthStatistics(string UID, string SqlWhere)
        {
            return DAL.UserFinanceLog.getUserMonthStatistics(UID, SqlWhere);
        }

        public static int UpdateStrField(string table, string filed, string filedvalue, string contion)
        {
            return DAL.UserFinanceLog.UpdateStrField(table, filed, filedvalue, contion);
        }

        public static int UserFinanceLogAdd(Model.UserFinanceLog userfinanceLog)
        {
            return DAL.UserFinanceLog.UserFinanceLogAdd(userfinanceLog);
        }

        public static int UserFinanceLogDelete(string ID)
        {
            return DAL.UserFinanceLog.UserFinanceLogDelete(ID);
        }

        public static int UserFinanceLogUpdateForIsState(string ID, string IsState)
        {
            return DAL.UserFinanceLog.UserFinanceLogUpdateForIsState(ID, IsState);
        }

        public static int UserWithdrawalsCancel(string ID)
        {
            return DAL.UserFinanceLog.UserWithdrawalsCancel(ID);
        }

        public static int YZM_ChongZhi_Success(int id)
        {
            return DAL.UserFinanceLog.YZM_ChongZhi_Success(id);
        }

        public static int YZM_TiXian_ShenQing(int uid, decimal amount, string financeInfo)
        {
            return DAL.UserFinanceLog.YZM_TiXian_ShenQing(uid, amount, financeInfo);
        }

        public static int YZM_TiXian_Success(int id)
        {
            return DAL.UserFinanceLog.YZM_TiXian_Success(id);
        }

        public static int YZM_TuiKuan_Success(string userID, decimal amount, string financeInfo)
        {
            return DAL.UserFinanceLog.YZM_TuiKuan_Success(userID, amount, financeInfo);
        }
    }
}

