using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

public partial class account_AddRole : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (this.Session["admin"] == null)
            {
                base.Response.Redirect("/account/Login.aspx");
            }
            if (BLL.GeneralMethods.GetPermissions(HttpContext.Current.Request.Url.ToString(), this.Session["admin"].ToString()))
            {
                base.Response.Redirect("/Index.aspx");
            }
            this.readSysTree(this.treeV_Authorize);
        }
    }

    public void AddChildNode(DataTable dt, TreeNode node, string id)
    {
        DataView view3 = new DataView(dt)
        {
            RowFilter = "parentid=" + id
        };
        DataView view = view3;
        foreach (DataRowView view2 in view)
        {
            TreeNode node3 = new TreeNode
            {
                Text = view2["name"].ToString(),
                Value = view2["id"].ToString()
            };
            TreeNode child = node3;
            //child.Expanded = true;
            node.ChildNodes.Add(child);
            this.AddChildNode(dt, child, view2["id"].ToString());
        }
    }

    protected void btn_ck_ServerClick(object sender, EventArgs e)
    {
        string name = this.name.Value.ToString();
        string isstate = this.isState.Items[this.isState.SelectedIndex].Value;
        try
        {
            if ((name.Length != 0) && (isstate.Length != 0))
            {
                string id = BLL.RoleList.setRole(name, isstate).ToString();
                if (id != "0")
                {
                    foreach (TreeNode node in this.treeV_Authorize.CheckedNodes)
                    {
                        BLL.RoleList.setMenuRIGHT(node.Value, id);
                    }
                    Commons.MessageBox.Show(this, "添加角色成功");
                    base.Response.Redirect("/account/RoleList.aspx");
                }
                else
                {
                    Commons.MessageBox.Show(this, "角色添加失败");
                }
            }
            else
            {
                Commons.MessageBox.Show(this, "请把信息填写完整");
            }
        }
        catch (Exception)
        {
            Commons.MessageBox.Show(this, "角色添加失败");
        }
    }

    public void readSysTree(TreeView TV)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("Menu", "", "*", "where 1=1");
        DataSet set = BLL.SelectRecord.SelectRecordData(selectRecord);
        if (set != null)
        {
            DataTable table = set.Tables[0];
            TV.Nodes.Clear();
            DataView view3 = new DataView(table)
            {
                RowFilter = "parentid=0"
            };
            DataView view = view3;
            foreach (DataRowView view2 in view)
            {
                TreeNode node2 = new TreeNode
                {
                    Text = view2["name"].ToString(),
                    Value = view2["id"].ToString()
                };
                TreeNode child = node2;
                TV.Nodes.Add(child);
                this.AddChildNode(table, child, view2["id"].ToString());
                //child.ExpandAll();
            }
        }
    }
}