using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.Services;

public partial class account_UserList : Page
{
    private DataTable dt;

    public DataTable BindData(string whereSql)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("view_RoleUser", "", "*", whereSql);
        this.dt = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        return this.dt;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!base.IsPostBack)
        {
            //Utility.RegisterTypeForAjax(typeof(AjaxMothodEdit));
            if (this.Session["admin"] == null)
            {
                base.Response.Redirect("/account/Login.aspx");
            }
            if (BLL.GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
            {
                base.Response.Redirect("/Index.aspx");
            }
            string whereSql = "where 1=1";
            this.GridView1.DataSource = this.BindData(whereSql);
            this.GridView1.DataBind();
        }
    }

    [WebMethod]
    public static bool btnDeleteUser(string id)
    {
        if (BLL.GeneralMethods.GeneralDelDB("Users", "where id=" + id).ToString() == "1" )
        {
            if (BLL.GeneralMethods.GeneralDelDB("RoleUsers", "where userid=" + id).ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}