using BLL;
using Commons;
using System;
using System.Web;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using System.Activities;
using Engine;
using WorkFlow;
using System.Activities.Presentation;
using System.Activities.DurableInstancing;
using DBAccess;
using System.Runtime.DurableInstancing;

public partial class document_EditDocument : System.Web.UI.Page
{
    private static string id = string.Empty;
    //ziyunhx add 2013-8-5 workflow Persistence
    WorkflowApplication instance = null;
    SqlWorkflowInstanceStore instanceStore;
    InstanceView view;
    //end

    protected void btnEdit_ServerClick(object sender, EventArgs e)
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

            //ziyunhx add 2013-8-5 workflow Persistence 
            //old code
            //WorkFlowTracking.instance = engineManager.createInstance(content, null, null);
            //WorkFlowTracking.instance.Run();
            //new code
            instance = engineManager.createInstance(content, null, null);
            if (instanceStore == null)
            {
                instanceStore = new SqlWorkflowInstanceStore(SqlHelper.strconn);
                view = instanceStore.Execute(instanceStore.CreateInstanceHandle(), new CreateWorkflowOwnerCommand(), TimeSpan.FromSeconds(30));
                instanceStore.DefaultInstanceOwner = view.InstanceOwner;
            }
            instance.InstanceStore = instanceStore;
            instance.Run();
            //end

            Model.Document documents = new Model.Document
            {
                ID = id.Trim(),
                Name = this.txtName.Value.Trim(),
                URL = this.uploadurl.Value.Trim(),
                Remark = this.txtReMark.Value.Trim(),
                WID = this.selWorkFlow.SelectedValue,
                WStep = "0",
                Result = "0",
                UID = this.Session["admin"].ToString(),
                //ziyunhx add 2013-8-5 workflow Persistence 
                //old code
                //FlowInstranceID = WorkFlowTracking.instance.Id,
                //new code
                FlowInstranceID = instance.Id,
                //end
            };

            if (BLL.Document.DocumentUpdate(documents) == 1)
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "编辑工作流" + documents.Name + "的信息成功");
                MessageBox.ShowAndRedirect(this, "编辑公文成功！", "/document/DocumentList.aspx");
            }
            else
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "编辑工作流" + documents.Name + "的信息失败");
                MessageBox.Show(this, "编辑工作流失败");
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
            if (base.Request.QueryString["id"] != null)
            {
                id = base.Request.QueryString["id"].ToString();

                Model.SelectRecord selectRecords = new Model.SelectRecord("WorkFlow", "", "*", "where State=1 order by id desc");
                DataTable tb = BLL.SelectRecord.SelectRecordData(selectRecords).Tables[0];
                if (tb.Rows.Count > 0)
                {
                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        ListItem li = new ListItem(tb.Rows[i][1].ToString(), tb.Rows[i][0].ToString());
                        this.selWorkFlow.Items.Add(li);
                    }
                }

                Model.SelectRecord selectRecord = new Model.SelectRecord("Document", "", "*", "where id='" + id + "'");
                DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                if (table.Rows.Count > 0)
                {
                    this.txtName.Value = table.Rows[0][1].ToString();                    
                    this.txtReMark.Value = table.Rows[0][3].ToString();
                    this.uploadurl.Value = table.Rows[0][2].ToString();
                    this.selWorkFlow.SelectedValue = table.Rows[0][4].ToString();
                }

            }
            else
            {
                MessageBox.Show(this, "请选择要编辑的公文！");
                base.Response.Redirect("/workflow/WorkFlowList.aspx");
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (UploadFile.HasFile)
        {
            string FileName = this.UploadFile.FileName;//获取上传文件的文件名,包括后缀
            string ExtenName = System.IO.Path.GetExtension(FileName);//获取扩展名
            string filenewname = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string SaveFileName = System.IO.Path.Combine(System.Web.HttpContext.Current.Request.MapPath("../upload/workflow/"), filenewname + ExtenName);//合并两个路径为上传到服务器上的全路径
            UploadFile.MoveTo(SaveFileName, Brettle.Web.NeatUpload.MoveToOptions.Overwrite);
            string url = "upload/workflow/" + filenewname + ExtenName; //文件保存的路径
            this.uploadurl.Value = url;
            //float FileSize = (float)System.Math.Round((float)UploadFile.ContentLength / 1024000, 1); //获取文件大小并保留小数点后一位,单位是M
            this.uploadfiles.Style["display"] = "none";
            this.uploadfileok.Style["display"] = "";
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
}