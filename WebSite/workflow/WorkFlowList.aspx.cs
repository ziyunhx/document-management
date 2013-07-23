using System;
using System.Data;
using System.Web;
using System.Web.Services;

public partial class workflow_WorkFlowList : System.Web.UI.Page
{
    private DataTable dt;

    public DataTable BindData(string whereSql)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("WorkFlow", "", "*", whereSql);
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