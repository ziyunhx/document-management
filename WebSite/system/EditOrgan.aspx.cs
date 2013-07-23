using BLL;
using Commons;
using System;
using System.Data;
using System.Web;

public partial class system_EditOrgan : System.Web.UI.Page
{
    private static string id = string.Empty;

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Model.Organizational Organ = new Model.Organizational
        {
            ID = id,
            Name = this.txtClassName.Text,
            PID = this.ddlRootID.SelectedValue.ToString(),
        };
        if (BLL.Organizational.OrganUpdate(Organ) == 1)
        {
            UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "组织架构管理", "修改ID为" + id + "的岗位信息");
            MessageBox.ResponseScript(this, "alert('编辑岗位信息成功！');parent.location.href='/system/OrganList.aspx'");
        }
        else
        {
            MessageBox.Show(this, "编辑岗位信息有误，请您重新操作");
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
            if ((base.Request.QueryString["id"].ToString() != null) && (base.Request.QueryString["id"].ToString().Trim().Length > 0))
            {
                id = base.Request.QueryString["id"].ToString();
            }
            Model.SelectRecord selectRecord = new Model.SelectRecord("Organizational", "", "*", "where ID=" + id);
            DataSet set = BLL.SelectRecord.SelectRecordData(selectRecord);
            this.txtClassName.Text = set.Tables[0].Rows[0]["Name"].ToString();
            BLL.Organizational.BindToDropDownList(this.ddlRootID, set.Tables[0].Rows[0]["PID"].ToString());
        }
    }
}