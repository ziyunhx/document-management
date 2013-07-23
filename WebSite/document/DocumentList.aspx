<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="DocumentList.aspx.cs" Inherits="document_DocumentList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>

        <table width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：公文管理 >>我的公文</td></tr>
		<tr><td class="doaction"><span><a href="/document/AddDocument.aspx">创建公文</a></span>
		</td></tr>
		<tr style="text-align:center;"><td>
            <asp:GridView ID="GridView1" runat="server" HeaderStyle-BackColor="Coral" Width="100%"  DataKeyNames="ID" BorderWidth="1px" BorderStyle="Dotted" CellPadding="3" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField HeaderText="编号">
                        <ItemTemplate>
                            <%# Eval("id") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="公文名">
                        <ItemTemplate>
                            <%# Eval("Name")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="工作流">
                        <ItemTemplate>
                            <%# Eval("workflowname")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="当前流转步骤">
                        <ItemTemplate>
                            <%# Eval("stepname").ToString() == "" ? "未流转" : Eval("stepname").ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="当前状态">
                        <ItemTemplate>
                            <%# getstatus(Eval("Result").ToString()) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="查看公文">
                        <ItemTemplate>
                            <a target="_blank" href="/<%# Eval("URL")%>">下载公文</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>                            
                            <a href="/document/EditDocument.aspx?id=<%# Eval("id").ToString()%>"><img src="../images/icon_edit.gif" alt="编辑公文" /></a> &nbsp;
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
		</td></tr>
	</table>
</asp:Content>

