using BLL;
using Commons;
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

public partial class account_AddUser : System.Web.UI.Page
{
    protected void btnAdd_ServerClick(object sender, EventArgs e)
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
            UserID = this.txtUserID.Value.Trim(),
            UserName = this.txtUserName.Value.Trim(),
            Telephone = this.txtTelephone.Text.Trim(),
            QQ = this.TextBox1.Text.Trim(),
            Password = Security.Encrypt(this.txtPassword.Value, "12345678"),
            IsState = "2",
            Email = this.txtEmail.Value,
            OID = s
        };
        if (((userInfo.UserID.Length == 0) || (userInfo.UserName.Length == 0)) || ((userInfo.QQ.Length == 0) || (userInfo.Email.Length == 0)))
        {
            MessageBox.Show(this, "请您填写完整的信息");
        }
        else
        {
            int i = BLL.Users.UserAdd(userInfo);
            if (i > 0)
            {
                BLL.Users.UserRoleAdd(i.ToString(), this.selIsState.SelectedValue);
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "会员管理", "创建会员" + userInfo.UserID + "用户名为" + userInfo.UserName + "的信息成功");
                MessageBox.ShowAndRedirect(this, "创建用户成功", "/account/UserList.aspx");
            }
            else
            {
                UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "会员管理", "创建会员" + userInfo.UserID + "用户名为" + userInfo.UserName + "的信息失败");
                MessageBox.Show(this, "创建用户失败");
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
            Model.SelectRecord selectRecord = new Model.SelectRecord("Role", "", "*", "where isState=1 order by id desc");
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ListItem li=new ListItem(table.Rows[i][1].ToString(),table.Rows[i][0].ToString());
                    this.selIsState.Items.Add(li);
                    if (i == 0)
                    {
                        this.selIsState.SelectedValue = table.Rows[0][0].ToString(); 
                    }
                }
            }
            BLL.Organizational.BindToListBox(List_Organ, "");
        }
    }
}