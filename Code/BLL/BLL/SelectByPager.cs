namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class SelectByPager
    {
        public static int GetCount(SelectField selectfield)
        {
            return DAL.SelectByPager.GetCount(selectfield);
        }

        public static string GetTotalNum(SelectField selectfield)
        {
            return DAL.SelectByPager.GetTotalNum(selectfield);
        }

        public static DataSet SelectByPagerData(Model.SelectByPager pager)
        {
            return DAL.SelectByPager.SelectByPagerData(pager);
        }

        public static DataSet SelectByPagerData(Model.SelectByPager pager, string strGroup)
        {
            return DAL.SelectByPager.SelectByPagerData(pager, strGroup);
        }
    }
}

