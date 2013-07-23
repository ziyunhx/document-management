using BLL;
using Commons;
using System;
using System.Web;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;

public partial class workflow_AuthorizeWorkFlow : System.Web.UI.Page
{
    private static string id = string.Empty;
    private static ArrayList str = new ArrayList();

    protected void btnAdd_ServerClick(object sender, EventArgs e)
    {
        //创建公文授权
        string addorgan = ",";
        for (int a1 = 0; a1 < this.List_ADDOrgan.Items.Count; a1++)
        {
            if (List_ADDOrgan.Items[a1].Selected == true)
            {
                addorgan += List_ADDOrgan.Items[a1].Value + ",";
            }
        }
        Model.WorkFlowRole addwork = new Model.WorkFlowRole
        {
            OID = addorgan,
            WID = id,
            WSTEP = "0",
            State = "1",
            name = "创建公文",
            value = "0",
        };
        BLL.WorkFlowRole.WorkFlowRoleAdd(addwork);

        //公文流转授权
        string qx = Request.Form["options"].ToString();
        string[] s = qx.Split(new char[] { ';' });
        for (int i = 0; i < s.Length-1; i++)
        {
            ArrayList temp = (ArrayList)str[i];
            Model.WorkFlowRole workrole = new Model.WorkFlowRole { 
                OID= ","+s[i].ToString(),
                WID = id,
                WSTEP = temp[1].ToString(),
                State = "1",
                name= temp[0].ToString(),
                value = temp[2].ToString(),
            };
            BLL.WorkFlowRole.WorkFlowRoleAdd(workrole);
        }

        //发布公文授权
        string publicorgan = ",";
        for (int a2 = 0; a2 < this.List_PublicOrgan.Items.Count; a2++)
        {
            if (List_PublicOrgan.Items[a2].Selected == true)
            {
                publicorgan += List_PublicOrgan.Items[a2].Value + ",";
            }
        }
        Model.WorkFlowRole publicwork = new Model.WorkFlowRole
        {
            OID = publicorgan,
            WID = id,
            WSTEP = "999",
            State = "1",
            name = "发布公文",
            value = "0",
        };
        BLL.WorkFlowRole.WorkFlowRoleAdd(publicwork);

        //查阅公文授权
        string readorgan = ",";
        for (int a3 = 0; a3 < this.List_Read.Items.Count; a3++)
        {
            if (List_Read.Items[a3].Selected == true)
            {
                readorgan += List_Read.Items[a3].Value + ",";
            }
        }
        Model.WorkFlowRole readwork = new Model.WorkFlowRole
        {
            OID = readorgan,
            WID = id,
            WSTEP = "1000",
            State = "1",
            name = "查阅公文",
            value = "0",
        };
        BLL.WorkFlowRole.WorkFlowRoleAdd(readwork);

        UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "工作流管理", "工作流授权" + "成功");
        MessageBox.ShowAndRedirect(this, "工作流活动授权成功！", "/workflow/WorkFlowList.aspx");
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
            if (base.Request.QueryString["wid"] != null)
            {
                id = base.Request.QueryString["wid"].ToString();
                BLL.Organizational.BindToListBox(List_ADDOrgan, "");
                BLL.Organizational.BindToListBox(List_PublicOrgan, "");
                BLL.Organizational.BindToListBox(List_Read, "");

                BLL.GeneralMethods.GeneralDelDB("WorkFlowRole", "where WID ='" + id + "'");
                //Model.SelectRecord selectRecord = new Model.SelectRecord("WorkFlowRole", "", "*", "where id='" + id + "'");
                //DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];

            }
            else
            {
                MessageBox.Show(this, "请选择要授权的工作流！");
                base.Response.Redirect("/workflow/WorkFlowList.aspx");
            }
        }
    }

    public string Initsh()
    {
        if (!IsPostBack)
        {
            StringBuilder builder = new StringBuilder();
            builder.Clear();
            str.Clear();
            Model.SelectRecord selectRecord = new Model.SelectRecord("WorkFlow", "", "*", "where id='" + id + "'");
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];

            // 读取文件的源路径及其读取流
            string strReadFilePath = System.Web.HttpContext.Current.Request.MapPath("/") + table.Rows[0][2].ToString();
            StreamReader srReadFile = new StreamReader(strReadFilePath);

            Regex r = new Regex("<a:DocActivity");
            Regex x = new Regex(@".*DisplayName=\""(?'name'[^\<]+)\""\s*sap.*StepID=\""(?'stepid'[^<]+)\"".*bookmarkName=\""(?'value'[^<]+)\""", RegexOptions.Compiled);
            // 读取流直至文件末尾结束
            while (!srReadFile.EndOfStream)
            {
                string strReadLine = srReadFile.ReadLine(); //读取每行数据
                Match m = r.Match(strReadLine);
                if (m.Success)
                {
                    MatchCollection mc = x.Matches(strReadLine);
                    ArrayList detail = new ArrayList();
                    foreach (Match ms in mc)
                    {
                        detail.Add(ms.Groups["name"].Value);
                        detail.Add(ms.Groups["stepid"].Value);
                        detail.Add(ms.Groups["value"].Value);
                    }
                    str.Add(detail);
                }
            }

            // 关闭读取流文件
            srReadFile.Close();

            for (int i = 0; i < str.Count; i++)
            {
                ArrayList content = (ArrayList)str[i];
                builder.Append("<tr><td>" + content[0].ToString() + "</td></tr><tr><td>");
                builder.Append("<select size=\"4\" name=\"contopt\" multiple=\"multiple\" id=\"" + content[2].ToString() + "\">");
                builder.Append(BLL.Organizational.BindToListBox("") + "</select></td><td><input type=\"radio\" value=\"0\" checked name=\"myrad" + i.ToString() + "\">审批<input type=\"radio\" value=\"1\" name=\"myrad" + i.ToString() + "\">会签</td></tr>");
            }
            return builder.ToString();
        }
        else
            return "";
    }
}