using BLL;
using Model;
using System;
using System.Data;
using System.Web;
using System.Web.Services;

public partial class system_OrganList : System.Web.UI.Page
{
    private SelectField field;
    public string pid = "0";

    public static string FormatIsState(string s, string ID)
    {
        if (s == "1")
        {
            return ("<img src='../images/yes.gif' onclick= 'javascript:UpdateOrganizationalIsState(" + ID + ",0);' />");
        }
        if (s == "0")
        {
            return ("<img src='../images/no.gif' onclick= 'javascript:UpdateOrganizationalIsState(" + ID + " ,1);' />");
        }
        return ("<img src='../images/no.gif' onclick= 'javascript:UpdateOrganizationalIsState(" + ID + " ,1);' />");
    }

    public static string ForMatRootName(string s)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("Organizational", "", "ID,Name,PID", "where ID = '" + s + "'");
        if (s != "0")
        {
            return BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0].Rows[0]["Name"].ToString();
        }
        return "顶级";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.Page.IsPostBack)
        {
            if (this.Session["admin"] == null)
            {
                base.Response.Redirect("/account/Login.aspx");
            }
            if (GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
            {
                base.Response.Redirect("/Index.aspx");
            }
            if ((base.Request.QueryString["pid"] != null) && (base.Request.QueryString["pid"].Trim() != ""))
            {
                this.pid = base.Request.QueryString["pid"].Trim();
            }
            Model.SelectRecord selectRecord = new Model.SelectRecord("Organizational", "", "ID,Name,PID", "where pid = '" + pid + "'");
            DataTable dt = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            this.gvClass.DataSource = dt;
            this.gvClass.DataBind();
        }
    }

    [WebMethod]
    public static bool btnDeleteOrgan(string id)
    {
        if (BLL.Organizational.OrganDelete(id) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}