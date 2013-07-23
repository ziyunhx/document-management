using BLL;
using Commons;
using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

public partial class account_EditUser : System.Web.UI.Page
{
    private static string id = string.Empty;

    protected void btnEdit_ServerClick(object sender, EventArgs e)
    {
        string s = "";
        for (int j = 0; j < List_Organ.Items.Count; j++)
        {
            if (List_Organ.Items[j].Selected == true)
                s += List_Organ.Items[j].Value.ToString() + ",";
        }
        s = s.TrimEnd(',');

        Model.Users userInfo = new Model.Users
        {
            UserID = this.txtUserID.Value,
            ID = id,
            UserName = this.txtUserName.Value,
            Telephone = this.txtTelephone.Value,
            QQ = this.TextBox1.Value,
            Email = this.txtEmail.Value,
            OID = s
        };
        if (this.txtPassword.Value.Trim().Length > 0)
        {
            if (this.Password1.Value != this.txtPassword.Value)
            {
                MessageBox.Show(this, "您两次输入的密码不一样，请重新输入");
                return;
            }
            userInfo.Password = Security.Encrypt(this.txtPassword.Value, "12345678");
        }
        if (((userInfo.UserID.Length == 0) || (userInfo.Email.Length == 0)))
        {
            MessageBox.Show(this, "请您填写完整的信息");
        }
        else
        {
            if (BLL.Users.UserUpdate(userInfo) == 1)
            {
                BLL.Users.UserRoleEdit(id, this.selIsState.SelectedValue);
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "会员管理", "编辑会员" + userInfo.UserID + "信息成功");
                MessageBox.ShowAndRedirect(this, "编辑成功！", "/account/UserList.aspx");
            }
            else
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "会员管理", "编辑会员" + userInfo.UserID + "信息失败");
                MessageBox.Show(this, "编辑失败！");
            }
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
            if (base.Request.QueryString["id"] != null)
            {
                id = base.Request.QueryString["id"].ToString();
                DataSet set = BLL.Users.SelectForUserID(id);
                this.txtEmail.Value = set.Tables[0].Rows[0]["Email"].ToString();
                this.txtTelephone.Value = set.Tables[0].Rows[0]["Telephone"].ToString();
                this.txtUserID.Value = set.Tables[0].Rows[0]["UserID"].ToString();
                this.txtUserName.Value = set.Tables[0].Rows[0]["UserName"].ToString();
                this.TextBox1.Value = set.Tables[0].Rows[0]["QQ"].ToString();
                this.selIsState.SelectedIndex = -1;
                Model.SelectRecord selectRecord = new Model.SelectRecord("Role", "", "*", "where isState=1 order by id desc");
                DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                if (table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {

                        ListItem li = new ListItem(table.Rows[i][1].ToString(), table.Rows[i][0].ToString());
                        this.selIsState.Items.Add(li);
                        Model.SelectRecord selectRecord2 = new Model.SelectRecord("RoleUsers", "", "*", "where userid=" + id);
                        DataTable dt = BLL.SelectRecord.SelectRecordData(selectRecord2).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            this.selIsState.SelectedValue = dt.Rows[0][1].ToString();
                        }
                    }
                }
                BLL.Organizational.BindToListBox(List_Organ, set.Tables[0].Rows[0]["OID"].ToString());
            }
            else
            {
                MessageBox.Show(this, "请选择要编辑的用户！");
                base.Response.Redirect("/account/UserList.aspx");
            }
        }
    }
}