using System;
using System.Data;
using System.IO;
using System.Text;

public partial class Main : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Session["admin"] != null && this.Session["IsState"] != null)
        {
            try
            {
                DataTable dt = BLL.Users.SelectForUserID(this.Session["admin"].ToString()).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    this.Lab_user.Text = "你好！" + dt.Rows[0]["UserName"].ToString() +
                        "，我要&nbsp;<a href=\"/account/login.aspx?logout=1\">退出系统</a>";
                }
                else
                {
                    this.Lab_user.Text = "你好！游客，我要&nbsp;<a href=\"/account/login.aspx\">登录系统</a>";
                }
            }
            catch (Exception exception)
            {
                base.Response.Write(exception.ToString());
            }
        }
        else{
            //this.Lab_user.Text ="你好！游客，我要&nbsp;<a href=\"/account/login.aspx\">登录系统</a>";
            base.Response.Redirect("/account/Login.aspx");
        }
    }

    public string InitMenu()
    {
        StringBuilder builder = new StringBuilder();
        Model.SelectRecord selectRecord = new Model.SelectRecord("view_UserPermissions", "", "name,url,id", "where parentid=0 and isState=1 and uid=" + this.Session["admin"].ToString() + " order by orderum");
        DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        int num = table.Rows.Count;
        if (num > 0)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (i == 0)
                {
                    builder.Append("<li class='first active'><a href='" + table.Rows[i]["url"].ToString() + "'>" + table.Rows[i]["name"].ToString() + "</a>");
                    builder.Append(this.InitMenu(Convert.ToInt32(table.Rows[i]["id"])));
                    builder.Append("</li>");
                }
                else if (i == num - 1)
                {
                    builder.Append("<li class='last'><a href='" + table.Rows[i]["url"].ToString() + "'>" + table.Rows[i]["name"].ToString() + "</a>");
                    builder.Append(this.InitMenu(Convert.ToInt32(table.Rows[i]["id"])));
                    builder.Append("</li>");
                }
                else
                {
                    builder.Append("<li><a href='" + table.Rows[i]["url"].ToString() + "'>" + table.Rows[i]["name"].ToString() + "</a>");
                    builder.Append(this.InitMenu(Convert.ToInt32(table.Rows[i]["id"])));
                    builder.Append("</li>");
                }
            }
        }
        return builder.ToString();
    }

    public string InitMenu(int parentid)
    {
        StringBuilder builder = new StringBuilder();
        
        Model.SelectRecord selectRecord = new Model.SelectRecord("view_UserPermissions", "", "name,url", string.Concat(new object[] { "where parentid=", parentid, " and isState=1 and display=1 and uid=", this.Session["admin"].ToString(), "order by orderum" }));
        DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        int num = table.Rows.Count;
        if (num > 0)
        {
            builder.Append("<ul>");
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (i == 0)
                {
                    builder.Append("<li class='first'><a href='" + table.Rows[i]["url"].ToString() + "'>" + table.Rows[i]["name"].ToString() + "</a></li>");
                }
                else if (i == num - 1)
                {
                    builder.Append("<li class='last'><a href='" + table.Rows[i]["url"].ToString() + "'>" + table.Rows[i]["name"].ToString() + "</a></li>");
                }
                else
                {
                    builder.Append("<li><a href='" + table.Rows[i]["url"].ToString() + "'>" + table.Rows[i]["name"].ToString() + "</a></li>");
                }
            }
            return (builder.ToString() + "</ul>");
        }
        else
        {
            return "";
        }
    }
}
