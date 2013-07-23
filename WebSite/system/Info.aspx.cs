using BLL;
using System;
using System.Data;
using System.Web;
using System.Text;

public partial class system_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Session["admin"] == null)
        {
            base.Response.Redirect("/account/Login.aspx");
        }
        if (GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
        {
            base.Response.Redirect("/Index.aspx");
        }
        if (IsPostBack)
        {
            try
            {
                Model.SelectRecord selectRecord = new Model.SelectRecord("SystemInfo", "", "id,name,value", "where 1=1");
                DataTable dt = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                for (int i = 1; i < 9; i++)
                {
                    if (Request.Form["input_" + i.ToString()] != null && Request.Form["input_" + i.ToString()].ToString()!=dt.Rows[i-1]["value"].ToString())
                    {
                        string value = Request.Form["input_" + i.ToString()].ToString();
                        BLL.SystemInfo.UpdateInfo(i.ToString(), value);
                    }
                }
                Commons.MessageBox.Show(this.Page,"修改系统信息成功!");
            }
            catch(Exception ex)
            {
                Commons.MessageBox.Show(this.Page, "修改系统信息失败!");    
            }
        }
    }

    public string GetSystemInfo()
    {
        try
        {
            StringBuilder builder = new StringBuilder();
            Model.SelectRecord selectRecord = new Model.SelectRecord("SystemInfo", "", "id,name,value", "where 1=1");
            DataTable dt = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if (dt.Rows.Count > 0)
            {
                builder.Append("<table>");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["id"].ToString() == "3" || dt.Rows[i]["id"].ToString() == "8")
                    {
                        builder.Append("<tr class='infotr'><td class='infoname'>" + dt.Rows[i]["name"].ToString() + "：</td><td><textarea name='input_" + dt.Rows[i]["id"].ToString() + "' id='input_" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["value"].ToString() + "</textarea></td></tr>");
                    }
                    else
                    {
                        builder.Append("<tr class='infotr'><td class='infoname'>" + dt.Rows[i]["name"].ToString() + "：</td><td><input type='text' name='input_" + dt.Rows[i]["id"].ToString() + "' id='input_" + dt.Rows[i]["id"].ToString() + "' value='" + dt.Rows[i]["value"].ToString() + "' /></td></tr>");
                    }
                }
            }
            return builder.ToString() + "</table>";
        }
        catch (Exception e)
        {
            Commons.MessageBox.Show(this.Page, "读取系统信息出错");
            return "";
        }
    }
}