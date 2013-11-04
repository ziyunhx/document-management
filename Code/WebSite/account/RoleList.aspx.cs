using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.Services;

public partial class Account_RoleList : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!base.IsPostBack)
        {
            //Ajax.Utility.RegisterTypeForAjax(typeof(AjaxMothodEdit));
            if (this.Session["admin"] == null)
            {
                base.Response.Redirect("/account/Login.aspx");
            }
            if (BLL.GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
            {
                base.Response.Redirect("/Index.aspx");
            }
            this.bindview();
        }
    }

    public void bindview()
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("Role", "", "*", "where 1=1");
        DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        this.GridView1.DataSource = table;
        this.GridView1.DataBind();
    }

    [WebMethod]
    public static bool btnDeleteRole(string id)
    {
        if (BLL.GeneralMethods.GeneralDelDB("Role", "where id=" + id).ToString() == "1")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}