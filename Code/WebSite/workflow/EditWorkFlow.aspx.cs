using BLL;
using Commons;
using System;
using System.Web;
using System.IO;
using System.Data;

public partial class workflow_EditWorkFlow : System.Web.UI.Page
{
    private static string id = string.Empty;

    protected void btnEdit_ServerClick(object sender, EventArgs e)
    {
        Model.WorkFlow workflows = new Model.WorkFlow
        {
            ID = id.Trim(),
            Name = this.txtName.Value.Trim(),
            URL = this.uploadurl.Value.Trim(),
            State = this.isState.Value.ToString(),
            Remark = this.txtReMark.Value.Trim(),
        };
        if (((workflows.Name.Length == 0) || (workflows.URL.Length == 0)) || (workflows.State.Length == 0))
        {
            MessageBox.Show(this, "请您填写完整的信息");
        }
        else
        {
            if (BLL.WorkFlow.WorkFlowUpdate(workflows) == 1)
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "编辑工作流" + workflows.Name + "的信息成功");
                MessageBox.ShowAndRedirect(this, "编辑工作流成功，请继续分配权限！", "/workflow/AuthorizeWorkFlow.aspx?wid="+id);
            }
            else
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "编辑工作流" + workflows.Name + "的信息失败");
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
                Model.SelectRecord selectRecord = new Model.SelectRecord("WorkFlow", "", "*", "where id='"+id+"'");
                DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                if (table.Rows.Count > 0)
                {
                    this.txtName.Value=table.Rows[0][1].ToString();
                    this.isState.Value=table.Rows[0][4].ToString();
                    this.txtReMark.Value=table.Rows[0][3].ToString();
                    this.uploadurl.Value = table.Rows[0][2].ToString();
                }
            }
            else
            {
                MessageBox.Show(this, "请选择要编辑的工作流！");
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