using BLL;
using Commons;
using System;
using System.Web;
using System.IO;

public partial class workflow_AddWorkFlow : System.Web.UI.Page
{
    protected void btnAdd_ServerClick(object sender, EventArgs e)
    {
        Model.WorkFlow workflows = new Model.WorkFlow
        {
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
            int Wid = BLL.WorkFlow.WorkFlowAdd(workflows);
            if (Wid > 0)
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "创建工作流" + workflows.Name + "的信息成功");
                MessageBox.ShowAndRedirect(this, "创建工作流成功,请对工作流活动授权！", "/workflow/AuthorizeWorkFlow.aspx?wid="+Wid);
            }
            else
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "创建工作流" + workflows.Name + "的信息失败");
                MessageBox.Show(this, "创建工作流失败");
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
                string SaveFileName = System.IO.Path.Combine(System.Web.HttpContext.Current.Request.MapPath("../upload/workflow/"), filenewname + ExtenName);//合并两个路径为上传到服务器上的全路径
                UploadFile.MoveTo(SaveFileName, Brettle.Web.NeatUpload.MoveToOptions.Overwrite);
                string url = "upload/workflow/" + filenewname + ExtenName; //文件保存的路径
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