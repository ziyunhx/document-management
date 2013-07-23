<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="EditOrgan.aspx.cs" Inherits="system_EditOrgan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
<table width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：组织架构管理  >> 岗位编辑</td></tr>
		<tr><td width="100%" valign="top" align="left">
			<table  class="abctab" width="100%" cellpadding="3" cellspacing="0" >
                            
						<tr><td class="abc" style="height: 26px">上级分类：</td><td class="abcright" style="height: 26px">
				    <asp:DropDownList ID="ddlRootID" runat="server"></asp:DropDownList>（顶级分类可不选）</td></tr>
				    
				<tr><td class="abc" style="height: 31px">分类名称：</td><td class="abcright" style="height: 31px">
				    <asp:TextBox ID="txtClassName" runat="server" Width="120px" onblur="ClassNameCheck();"></asp:TextBox></td></tr>  
 
				<tr><td class="abc">&nbsp;</td><td class="abcright">
                    <asp:Button ID="btnAdd" runat="server" Text="编 辑" class="sbutton" OnClick="btnAdd_Click" /></td></tr>
		  </table>
			</td></tr>
	</table>
</asp:Content>

