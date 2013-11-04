using System;
using System.Data;
using Commons;

public partial class Account_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Model.SelectRecord selectRecord = new Model.SelectRecord("SystemInfo", "", "value", "where 1=1");
            DataTable dt = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if (dt.Rows.Count > 0)
            {
                this.lab_address.Text = dt.Rows[3][0].ToString();
                this.lab_phone.Text = dt.Rows[4][0].ToString();
                this.lab_website.Text = dt.Rows[5][0].ToString();
                this.lab_email.Text = dt.Rows[6][0].ToString();
                this.lab_remark.Text = dt.Rows[7][0].ToString();
                this.email.HRef = "mailto:" + dt.Rows[6][0].ToString();
                this.website.HRef = "http://" + dt.Rows[5][0].ToString();
            }
        }
        catch (Exception ex)
        {
            //数据库连接错误，无法读取信息
            base.Response.Write(ex.ToString());
        }
    }

    protected void btnLogin_ServerClick(object sender, EventArgs e)
    {
        if (this.txtUserId.Text.Contains("'"))
        {
            this.lbl_Message.Text = "不能包含特殊字符";
        }
        else
        {
            DataSet set = BLL.Users.UserLogin(this.txtUserId.Text.Trim(), Security.Encrypt(this.txtPwd.Text.Trim(), "12345678"));
            if (set.Tables[0].Rows.Count > 0)
            {
                if (((int)set.Tables[0].Rows[0]["IsState"]) > 1)
                {
                    this.Session["admin"] = set.Tables[0].Rows[0]["ID"].ToString().Trim();
                    this.Session["IsState"] = set.Tables[0].Rows[0]["IsState"].ToString().Trim();
                    base.Response.Redirect("/Index.aspx");
                }
                else
                {
                    this.lbl_Message.Text = "该用户不是正常用户，请跟管理员联系";
                }
            }
            else
            {
                this.lbl_Message.Text = "用户名或密码错误！";
            }
        }
    }
}
