<%@ Page Title="工作流列表" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="WorkFlowList.aspx.cs" Inherits="workflow_WorkFlowList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
    <script language="JavaScript" src="/scripts/getids.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function DelWorkFlow(id) {
            if (confirm('确定要删除吗？')) {
                PageMethods.btnDeleteWorkFlow(id);
                alert("删除成功！");
                location.reload();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>

        <table width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：工作流管理 >>工作流列表</td></tr>
		<tr><td class="doaction"><span><a href="/workflow/AddWorkFlow.aspx">新增工作流</a></span>
		</td></tr>
		<tr style="text-align:center;"><td>
            <asp:GridView ID="GridView1" runat="server" HeaderStyle-BackColor="Coral" Width="100%"  DataKeyNames="ID" BorderWidth="1px" BorderStyle="Dotted" CellPadding="3" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField HeaderText="编号">
                        <ItemTemplate>
                            <%# Eval("id") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="工作流名">
                        <ItemTemplate>
                            <%# Eval("Name")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="WorkFlow文件">
                        <ItemTemplate>
                            <%# Eval("URL")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="当前状态">
                        <ItemTemplate>
                            <%# Eval("State").ToString() == "1" ? "<img src='../images/yes.gif'>" : "<img src='../images/no.gif'>" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>    
                            <a href="/workflow/EditWorkFlow.aspx?id=<%# Eval("id").ToString()%>"><img src="../images/icon_edit.gif" alt="编辑工作流" /></a> &nbsp;              
                            <a href="#" onclick="DelWorkFlow(<%#Eval("id")%>);"><img src="../images/no.gif" alt="删除工作流" /></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
		</td></tr>
	</table>
</asp:Content>

