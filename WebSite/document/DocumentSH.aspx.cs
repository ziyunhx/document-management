using BLL;
using Commons;
using System;
using System.Web;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using System.Activities;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WorkFlow;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Activities.Presentation.Services;
using System.Activities.Core.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Statements;
using System.Activities.Presentation.Metadata;
using System.Reflection;

public partial class document_DocumentSH : System.Web.UI.Page
{
    private static string id = string.Empty;

    protected void btnEdit_ServerClick(object sender, EventArgs e)
    {
        if (this.DropDown_sp.SelectedValue.Length == 0) 
        {
            MessageBox.Show(this, "请您选择审核结果");
        }
        else
        {
            Model.SelectRecord selectRecords = new Model.SelectRecord("view_DocumentInfo", "", "*", "where id='" + id + "'");
            DataTable dt = BLL.SelectRecord.SelectRecordData(selectRecords).Tables[0];
                     
            if (this.DropDown_sp.SelectedValue == "true")
            {
                string[] s = dt.Rows[0]["OID"].ToString().Split(new char[] { ',' });
                if (s[s.Length - 1] == "1")
                {
                    Model.SelectRecord selectExecut = new Model.SelectRecord("WorkFlowExecution", "", "*", "where DID='" + id + "' and step ='" + dt.Rows[0]["WStep"].ToString() + "' and UID='"+this.Session["admin"].ToString()+"'");
                    DataTable executdt = BLL.SelectRecord.SelectRecordData(selectExecut).Tables[0];
                    if (executdt.Rows.Count > 0)
                    {
                        MessageBox.ShowAndRedirect(this, "该公文您已审核，请等待其他人审核！", "/document/DocumentList.aspx");
                    }
                    else {
                        Model.SelectRecord selectExecutCount = new Model.SelectRecord("WorkFlowExecution", "", "distinct UID", "where DID='" + id + "' and step ='" + dt.Rows[0]["WStep"].ToString() + "'");
                        DataTable dtcount = BLL.SelectRecord.SelectRecordData(selectExecutCount).Tables[0];

                        string where = "where IsState=2 AND (1=2 ";

                        string[] str = dt.Rows[0]["OID"].ToString().Split(new char[] { ',' });
                        for (int j = 1; j < str.Length-1; j++)
                        {
                            where += "OR OID like '%" + str[j].ToString() + "%' ";
                        }
                        where += ") order by id desc";

                        Model.SelectRecord selectUserCount = new Model.SelectRecord("Users", "", "ID", where);
                        DataTable usercount = BLL.SelectRecord.SelectRecordData(selectUserCount).Tables[0];

                        if (dtcount.Rows.Count + 1 == usercount.Rows.Count)
                        {
                            if (WorkFlowTracking.instance.GetBookmarks().Count(p => p.BookmarkName == dt.Rows[0]["value"].ToString()) == 1)
                            {
                                WorkFlowTracking.instance.ResumeBookmark(dt.Rows[0]["value"].ToString(), this.DropDown_sp.SelectedValue.ToString());
                            }
                        }

                    }
                }
                else
                {
                    //一旦审批未通过，删除该公文当前流转步骤信息
                    BLL.GeneralMethods.GeneralDelDB("WorkFlowExecution", "where DID ='" + id + "' and step='" + dt.Rows[0]["WStep"].ToString() + "'");
                    if (WorkFlowTracking.instance.GetBookmarks().Count(p => p.BookmarkName == dt.Rows[0]["value"].ToString()) == 1)
                    {
                        WorkFlowTracking.instance.ResumeBookmark(dt.Rows[0]["value"].ToString(), this.DropDown_sp.SelectedValue.ToString());
                    }
                }

                Model.WorkFlowExecution workexe = new Model.WorkFlowExecution
                {
                    DID = dt.Rows[0]["ID"].ToString(),
                    UID = this.Session["admin"].ToString(),
                    step = dt.Rows[0]["WStep"].ToString(),
                    Remark = this.txtSP.Value.Trim(),
                    Result = "1",
                };

                BLL.WorkFlowExecution.Add(workexe);
            }
            else
            {
                Model.WorkFlowExecution workexe = new Model.WorkFlowExecution
                {
                    DID = dt.Rows[0]["ID"].ToString(),
                    UID = this.Session["admin"].ToString(),
                    step = dt.Rows[0]["WStep"].ToString(),
                    Remark = this.txtSP.Value.Trim(),
                    Result = "2",
                };

                BLL.WorkFlowExecution.Add(workexe);

                if (WorkFlowTracking.instance.GetBookmarks().Count(p => p.BookmarkName == dt.Rows[0]["value"].ToString()) == 1)
                {
                    WorkFlowTracking.instance.ResumeBookmark(dt.Rows[0]["value"].ToString(), this.DropDown_sp.SelectedValue.ToString());
                }
            }
            UserOperatingManager.InputUserOperating(this.Session["admin"].ToString(), "公文管理", "公文审核" + dt.Rows[0]["Name"].ToString() + "的信息成功");
            MessageBox.ShowAndRedirect(this, "审核公文成功！", "/document/DocumentList.aspx");
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

                Model.SelectRecord selectRecord = new Model.SelectRecord("Document", "", "*", "where id='" + id + "'");
                DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                if (table.Rows.Count > 0)
                {
                    this.txtName.Text = table.Rows[0][1].ToString();
                    this.txtReMark.Value = table.Rows[0][3].ToString();
                }

                Model.SelectRecord selectRecords = new Model.SelectRecord("WorkFlowRole", "", "*", "where WID='" + table.Rows[0]["WID"].ToString() + "' AND WStep < '" + table.Rows[0]["WStep"].ToString() + "' and WStep > 0 order by id desc");
                DataTable tb = BLL.SelectRecord.SelectRecordData(selectRecords).Tables[0];
                ListItem litrue = new ListItem("同意" , "true");
                ListItem lifalse = new ListItem("不同意", "false");
                this.DropDown_sp.Items.Add(litrue);
                this.DropDown_sp.Items.Add(lifalse);

                if (tb.Rows.Count > 0)
                {
                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        ListItem li = new ListItem("退回" + tb.Rows[i]["name"].ToString(), tb.Rows[i]["value"].ToString());
                        this.DropDown_sp.Items.Add(li);
                    }
                }

            }
            else
            {
                MessageBox.Show(this, "请选择要审核的公文！");
                base.Response.Redirect("/workflow/WorkFlowList.aspx");
            }
        }
    }
}