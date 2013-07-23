<%@ Page Title="用户列表" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="UserList.aspx.cs" Inherits="account_UserList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
    <script language="JavaScript" src="/scripts/getids.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function DelUser(id) {
            if (confirm('确定要删除吗？')) {
                if (id != "1") {
                    PageMethods.btnDeleteUser(id)
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
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：用户管理 >>用户列表</td></tr>
		<tr><td class="doaction"><span><a href="/account/RoleList.aspx">角色列表</a></span>
		&nbsp;&nbsp;&nbsp;&nbsp;<span><a href="/account/AddUser.aspx">添加用户</a></span>
		</td></tr>
		<tr style="text-align:center;"><td>
            <asp:GridView ID="GridView1" runat="server" HeaderStyle-BackColor="Coral" Width="100%"  DataKeyNames="ID" BorderWidth="1px" BorderStyle="Dotted" CellPadding="3" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField HeaderText="用户名">
                        <ItemTemplate>
                            <%# Eval("userid")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="姓名">
                        <ItemTemplate>
                            <%# Eval("UserName")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="E-mail">
                        <ItemTemplate>
                            <%# Eval("Email")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="所属角色">
                        <ItemTemplate>
                            <%# Eval("name")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="当前岗位">
                        <ItemTemplate>
                            <%# Eval("OID") %>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="当前状态">
                        <ItemTemplate>
                            <%# Eval("IsState").ToString() == "1" ? "<img src='../images/yes.gif'>" : "<img src='../images/no.gif'>" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>    
                            <a href="/account/EditUser.aspx?id=<%# Eval("id").ToString()%>"><img src="../images/icon_edit.gif" alt="编辑用户" /></a> &nbsp;              
                            <a href="#" onclick="DelUser(<%#Eval("id")%>);"><img src="../images/no.gif" alt="删除用户" /></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
		</td></tr>
	</table>
</asp:Content>

