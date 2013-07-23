using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (((base.Request.QueryString["logout"] != null) && (base.Request.QueryString["logout"].ToString() != "")) && (base.Request.QueryString["logout"].ToString() == "1"))
        {
            this.Session.Remove("admin");
            base.Response.Redirect("/account/Login.aspx");
        }
        else if (this.Session["admin"] != null && this.Session["IsState"] != null)
        {
            base.Response.Redirect("/Index.aspx");
        }
    }
}
