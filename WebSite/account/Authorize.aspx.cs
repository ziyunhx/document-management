using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class account_Authorize : Page, IRequiresSessionState
{
    private ArrayList list;


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
            string id = base.Request.QueryString["id"].ToString();
            this.GetRoleInfo(id);
            this.list = this.getMenuRIGHT(id);
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
            if (this.list.Contains(view2["id"].ToString()))
            {
                child.Checked = true;
            }
            child.Expanded = true;
            node.ChildNodes.Add(child);
            this.AddChildNode(dt, child, view2["id"].ToString());
        }
    }

    protected void btn_ck_ServerClick(object sender, EventArgs e)
    {
        string id = this.rid.Value;
        string name = this.name.Value;
        string isstate = this.isState.Items[this.isState.SelectedIndex].Value;
        try
        {
            BLL.RoleList.UpdateRole(id, name, isstate);
            BLL.RoleList.DelMenuRIGHT(id);
            foreach (TreeNode node in this.treeV_Authorize.CheckedNodes)
            {
                BLL.RoleList.setMenuRIGHT(node.Value, id);
            }
            Commons.MessageBox.Show(this, "角色编辑成功");
            base.Response.Redirect("/account/RoleList.aspx");
        }
        catch (Exception)
        {
            Commons.MessageBox.Show(this, "角色修改失败");
        }
    }

    private void GetRoleInfo(string id)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("Role", "", "*", " where id=" + id);
        DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        this.rid.Value = table.Rows[0]["id"].ToString();
        this.name.Value = table.Rows[0]["name"].ToString();
        if (table.Rows[0]["isState"].ToString() == "1")
        {
            this.isState.SelectedIndex = 0;
        }
        else
        {
            this.isState.SelectedIndex = 1;
        }
    }

    public ArrayList getMenuRIGHT(string id)
    {
        Model.SelectRecord selectRecord = new Model.SelectRecord("MenuRIGHT", "", "*", "where roleid=" + id);
        DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
        ArrayList list = new ArrayList();
        for (int i = 0; i < table.Rows.Count; i++)
        {
            list.Add(table.Rows[i]["flowid"].ToString());
        }
        return list;
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
                if (this.list.Contains(view2["id"].ToString()))
                {
                    child.Checked = true;
                }
                TV.Nodes.Add(child);
                this.AddChildNode(table, child, view2["id"].ToString());
                child.ExpandAll();
            }
        }
    }
}