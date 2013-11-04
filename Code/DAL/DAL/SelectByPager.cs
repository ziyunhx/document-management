namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class SelectByPager
    {
        public static int GetCount(SelectField selectfield)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@TableName", SqlDbType.VarChar, 100), new SqlParameter("@Field", SqlDbType.VarChar, 100), new SqlParameter("@NewField", SqlDbType.VarChar, 100), new SqlParameter("@Condition", SqlDbType.VarChar, 0x1f40) };
            pars[0].Value = selectfield.TableName;
            pars[1].Value = selectfield.Field;
            pars[2].Value = selectfield.NewField;
            pars[3].Value = selectfield.Condition;
            return (int) SqlHelper.GetAllInfo(pars, "SelectField").Tables[0].Rows[0][0];
        }

        public static string GetTotalNum(SelectField selectfield)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@TableName", SqlDbType.VarChar, 100), new SqlParameter("@Field", SqlDbType.VarChar, 100), new SqlParameter("@NewField", SqlDbType.VarChar, 100), new SqlParameter("@Condition", SqlDbType.VarChar, 0x1f40) };
            pars[0].Value = selectfield.TableName;
            pars[1].Value = selectfield.Field;
            pars[2].Value = selectfield.NewField;
            pars[3].Value = selectfield.Condition;
            DataSet allInfo = SqlHelper.GetAllInfo(pars, "getTotalNum");
            if ((allInfo != null) && (allInfo.Tables[0].Rows.Count != 0))
            {
                return allInfo.Tables[0].Rows[0][0].ToString();
            }
            return "0";
        }

        public static DataSet SelectByPagerData(Model.SelectByPager pager)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@col", SqlDbType.VarChar, 100), new SqlParameter("@Columnlist", SqlDbType.VarChar, 500), new SqlParameter("@pagesize", SqlDbType.Int), new SqlParameter("@pageindex", SqlDbType.Int), new SqlParameter("@docount", SqlDbType.Bit), new SqlParameter("@where", SqlDbType.VarChar, 0x1f40), new SqlParameter("@order", SqlDbType.VarChar, 100), new SqlParameter("@tabs", SqlDbType.VarChar, 100) };
            pars[0].Value = pager.Col;
            pars[1].Value = pager.Columnlist;
            pars[2].Value = pager.Pagesize;
            pars[3].Value = pager.PageIndex;
            pars[4].Value = pager.DoCount;
            pars[5].Value = pager.Where;
            pars[6].Value = pager.Order;
            pars[7].Value = pager.Tabs;
            return SqlHelper.GetAllInfo(pars, "sp_SelectPager");
        }

        public static DataSet SelectByPagerData(Model.SelectByPager pager, string strGroup)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@col", SqlDbType.VarChar, 100), new SqlParameter("@Columnlist", SqlDbType.VarChar, 500), new SqlParameter("@pagesize", SqlDbType.Int), new SqlParameter("@pageindex", SqlDbType.Int), new SqlParameter("@docount", SqlDbType.Bit), new SqlParameter("@where", SqlDbType.VarChar, 0x1f40), new SqlParameter("@order", SqlDbType.VarChar, 100), new SqlParameter("@tabs", SqlDbType.VarChar, 100), new SqlParameter("@group", SqlDbType.VarChar, 100) };
            pars[0].Value = pager.Col;
            pars[1].Value = pager.Columnlist;
            pars[2].Value = pager.Pagesize;
            pars[3].Value = pager.PageIndex;
            pars[4].Value = pager.DoCount;
            pars[5].Value = pager.Where;
            pars[6].Value = pager.Order;
            pars[7].Value = pager.Tabs;
            pars[8].Value = strGroup;
            return SqlHelper.GetAllInfo(pars, "sp_SelectPagerGroupBy");
        }
    }
}

