namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class Document
    {
        public static int DocumentAdd(Model.Document documentinfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@Name", SqlDbType.VarChar, 50), new SqlParameter("@URL", SqlDbType.VarChar, 100), new SqlParameter("@Remark", SqlDbType.VarChar, 255), new SqlParameter("@WID", SqlDbType.Int), new SqlParameter("@WStep", SqlDbType.Int), new SqlParameter("@Result", SqlDbType.TinyInt), new SqlParameter("@UID", SqlDbType.VarChar, 30), new SqlParameter("@FlowInstranceID", SqlDbType.UniqueIdentifier) };
            pars[0].Value = documentinfo.Name;
            pars[1].Value = documentinfo.URL;
            pars[2].Value = documentinfo.Remark;
            pars[3].Value = documentinfo.WID;
            pars[4].Value = documentinfo.WStep;
            pars[5].Value = documentinfo.Result;
            pars[6].Value = documentinfo.UID;
            pars[7].Value = documentinfo.FlowInstranceID;
            return SqlHelper.ExecuteProcess("pro_Document_Add", pars);
        }

        public static int DocumentDel(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_Document_Del", pars);
        }

        public static int PublicDocument(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_Document_Public", pars);
        }

        public static int DocumentUpdate(Model.Document documentinfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@Name", SqlDbType.VarChar, 50), new SqlParameter("@URL", SqlDbType.VarChar, 100), new SqlParameter("@Remark", SqlDbType.VarChar, 255), new SqlParameter("@WID", SqlDbType.Int), new SqlParameter("@WStep", SqlDbType.Int), new SqlParameter("@Result", SqlDbType.TinyInt), new SqlParameter("@FlowInstranceID", SqlDbType.UniqueIdentifier) };
            pars[0].Value = documentinfo.ID;
            pars[1].Value = documentinfo.Name;
            pars[2].Value = documentinfo.URL;
            pars[3].Value = documentinfo.Remark;
            pars[4].Value = documentinfo.WID;
            pars[5].Value = documentinfo.WStep;
            pars[6].Value = documentinfo.Result;
            pars[7].Value = documentinfo.FlowInstranceID;
            return SqlHelper.ExecuteProcess("pro_Document_Update", pars);
        }

        public static int DocumentEndStep(Guid FlowID, string result)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@FlowInstranceID", SqlDbType.UniqueIdentifier), new SqlParameter("@Result", SqlDbType.Int) };
            pars[0].Value = FlowID;
            pars[1].Value = result;
            return SqlHelper.ExecuteProcess("pro_Document_EndStep", pars);
        }

        public static int DocumentStep(Guid FlowID, string StepID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@FlowInstranceID", SqlDbType.UniqueIdentifier), new SqlParameter("@WStep", SqlDbType.Int) };
            pars[0].Value = FlowID;
            pars[1].Value = StepID;
            return SqlHelper.ExecuteProcess("pro_Document_Step", pars);
        }
    }
}

