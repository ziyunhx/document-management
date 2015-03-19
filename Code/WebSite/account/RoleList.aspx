<%@ Page Title="角色列表" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="RoleList.aspx.cs" Inherits="Account_RoleList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
    <script language="JavaScript" src="/scripts/getids.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function DelAuthorize(id) {
            if (confirm('确定要删除吗？')) {
                if (id != "1") {
                    PageMethods.btnDeleteRole(id)
                    alert("删除成功！");
                    location.reload();
                }
                else {
                    alert("删除失败！");
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>
    <table width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100%" height="26" class="leftmenu">当前位置：用户管理 >>角色列表</td></tr>
		<tr><td class="doaction"><span><a href="/account/UserList.aspx">用户列表</a></span>
		&nbsp;&nbsp;&nbsp;&nbsp;<span><a href="/account/AddRole.aspx">添加角色</a></span>
		</td></tr>
		<tr style="text-align:center;"><td>
            <asp:GridView ID="GridView1" runat="server" HeaderStyle-BackColor="Coral" Width="100%"  DataKeyNames="ID" BorderWidth="1px" BorderStyle="Dotted" CellPadding="3" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField HeaderText="角色编号">
                        <ItemTemplate>
                            <%# Eval("id") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="角色名称">
                        <ItemTemplate>
                            <%# Eval("name") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="当前状态">
                        <ItemTemplate>
                            <%# Eval("IsState").ToString() == "1" ? "<img src='../images/yes.gif'>" : "<img src='../images/no.gif'>"%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>    
                            <a href="/account/Authorize.aspx?id=<%# Eval("id").ToString()%>"><img src="../images/icon_edit.gif" alt="编辑角色" style="cursor:hand;" onclick="ShowPage(<%#Eval("id")%>);" /></a> &nbsp;
                            <a href="#" onclick='DelAuthorize(<%# Eval("id").ToString()%>);'> <img alt="删除角色" src="../images/no.gif" /></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
		</td></tr>
	</table>
</asp:Content>

