using BLL;
using Commons;
using System;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Activities.Presentation.Services;
using System.Activities;
using System.Activities.Debugger;
using System.Collections.Generic;
using System.Activities.Tracking;
using WorkFlow;
using Engine;
using System.Activities.Presentation;

public partial class document_AddDocument : System.Web.UI.Page
{
    protected void btnAdd_ServerClick(object sender, EventArgs e)
    {
        if (((this.txtName.Value.Length == 0) || (this.uploadurl.Value.Length == 0)) || (this.selWorkFlow.SelectedValue.Length == 0))
        {
            MessageBox.Show(this, "请您填写完整的信息");
        }
        else
        {
            Model.SelectRecord selectRecord = new Model.SelectRecord("WorkFlow", "", "*", "where id='" + this.selWorkFlow.SelectedValue + "'");
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            string content = File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("../") + table.Rows[0]["URL"].ToString());
            WorkFlowTracking.instance = engineManager.createInstance(content, null, null);
            WorkFlowTracking.instance.Run();

            Model.Document documents = new Model.Document
            {
                Name = this.txtName.Value.Trim(),
                URL = this.uploadurl.Value.Trim(),
                Remark = this.txtReMark.Value.Trim(),
                WID = this.selWorkFlow.SelectedValue,
                WStep = "0",
                Result = "0",
                UID = this.Session["admin"].ToString(),
                FlowInstranceID = WorkFlowTracking.instance.Id,
            };

            int s = BLL.Document.DocumentAdd(documents);

            if (s > 0)
            {
                Model.WorkFlowExecution workexe = new Model.WorkFlowExecution
                {
                    DID = s.ToString(),
                    UID = this.Session["admin"].ToString(),
                    step = "0",
                    Remark = "",
                    Result = "1",
                };

                BLL.WorkFlowExecution.Add(workexe);

                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "公文管理", "创建公文" + documents.Name + "的信息成功");
                MessageBox.ShowAndRedirect(this, "创建公文成功", "/document/DocumentList.aspx");
            }
            else
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "公文管理", "创建公文" + documents.Name + "的信息失败");
                MessageBox.Show(this, "创建公文失败");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!base.IsPostBack)
        {
            if (this.Session["admin"] == null)
            {
                base.Response.Redirect("/account/Login.aspx");
            }
            if (GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
            {
                base.Response.Redirect("/Index.aspx");
            }

            string where = "where State=1 AND (1=2 ";

            DataTable dt = BLL.Users.SelectForUserID(this.Session["admin"].ToString()).Tables[0];
            string[] str = dt.Rows[0]["OID"].ToString().Split(new char[]{','});
            for (int j = 0; j < str.Length; j++)
            {
                where += "OR OID like '%,"+str[j].ToString()+",%' ";
            }

            where += ") order by id desc";

            Model.SelectRecord selectRecord = new Model.SelectRecord("view_AddDocument", "", "*", where);
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ListItem li = new ListItem(table.Rows[i][1].ToString(), table.Rows[i][0].ToString());
                    this.selWorkFlow.Items.Add(li);
                    if (i == 0)
                    {
                        this.selWorkFlow.SelectedValue = table.Rows[0][0].ToString();
                    }
                }
            }
            else
            {
                MessageBox.ShowAndRedirect(this, "您不具有创建公文的权限", "/Index.aspx");
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (UploadFile.HasFile)
        {
            try
            {
                string FileName = this.UploadFile.FileName;//获取上传文件的文件名,包括后缀
                string ExtenName = System.IO.Path.GetExtension(FileName);//获取扩展名
                string filenewname = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string SaveFileName = System.IO.Path.Combine(System.Web.HttpContext.Current.Request.MapPath("../upload/document/"), filenewname + ExtenName);//合并两个路径为上传到服务器上的全路径
                UploadFile.MoveTo(SaveFileName, Brettle.Web.NeatUpload.MoveToOptions.Overwrite);
                string url = "upload/document/" + filenewname + ExtenName; //文件保存的路径
                this.uploadurl.Value = url;
                //float FileSize = (float)System.Math.Round((float)UploadFile.ContentLength / 1024000, 1); //获取文件大小并保留小数点后一位,单位是M
                this.uploadfiles.Style["display"] = "none";
                this.uploadfileok.Style["display"] = "";
            }
            catch
            {
                this.uploadfiles.Style["display"] = "none";
                this.uploadfilefalse.Style["display"] = "";
            }
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        this.uploadfiles.Style["display"] = "";
        this.uploadfileok.Style["display"] = "none";
        string pSavedPath1 = System.Web.HttpContext.Current.Request.MapPath("../") + this.uploadurl.Value;
        if (File.Exists(pSavedPath1))
        {
            FileInfo fi = new FileInfo(pSavedPath1);
            if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                fi.Attributes = FileAttributes.Normal;
            File.Delete(pSavedPath1);
        }
        this.uploadurl.Value = "";
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        this.uploadfiles.Style["display"] = "";
        this.uploadfilefalse.Style["display"] = "none";
    }
}