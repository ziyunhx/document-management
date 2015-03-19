namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class SelectRecord
    {
        public static DataSet SelectRecordData(Model.SelectRecord selectRecord)
        {
            return DAL.SelectRecord.SelectRecordData(selectRecord);
        }
    }
}

