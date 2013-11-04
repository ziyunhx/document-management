using System;
using System.Data;
using System.Web;
using System.Web.Services;

public partial class document_DocumentList : System.Web.UI.Page
{
    private DataTable dt;

    public DataTable BindData(string whereSql)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("view_DocumentInfo", "", "*", whereSql);
        this.dt = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        return this.dt;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!base.IsPostBack)
        {
            if (this.Session["admin"] == null)
            {
                base.Response.Redirect("/account/Login.aspx");
            }
            if (BLL.GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
            {
                base.Response.Redirect("/Index.aspx");
            }
            string whereSql = "where UID='" + this.Session["admin"].ToString() + "'";
            this.GridView1.DataSource = this.BindData(whereSql);
            this.GridView1.DataBind();
        }
    }

    public string getstatus(string sid)
    {
        if (sid == "0")
        {
            return "等待审核";
        }
        else if (sid == "1")
        {
            return "通过审核";
        }
        else if (sid == "2")
        {
            return "未通过审核";
        }
        else if (sid == "3")
        {
            return "已发布";
        }
        else
        {
        return "";
        }
    }

    [WebMethod]
    public static bool btnDeleteWorkFlow(string id)
    {
        if (BLL.WorkFlow.WorkFlowDel(id) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}