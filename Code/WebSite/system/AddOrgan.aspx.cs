using BLL;
using Commons;
using System;
using System.Web;

public partial class system_AddOrgan : System.Web.UI.Page
{
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (this.txtClassName.Text.Length == 0)
        {
            MessageBox.Show(this, "请填写完整的网站类别信息");
        }
        else
        {
            Model.Organizational Organ = new Model.Organizational
            {
                Name = this.txtClassName.Text,
                PID = this.ddlRootID.SelectedValue.ToString(),
            };
            if (BLL.Organizational.OrganAdd(Organ) > 0)
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "组织结构管理", "添加了一个新的岗位，名称为：" + this.txtClassName.Text);
                MessageBox.ResponseScript(this, "alert('添加岗位信息成功！');parent.location.href='/system/OrganList.aspx'");
            }
            else
            {
                MessageBox.Show(this, "创建岗位信息有误，请您重新操作");
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
            BLL.Organizational.BindToDropDownList(this.ddlRootID, "0");
        }
    }

}