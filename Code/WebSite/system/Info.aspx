<%@ Page Title="系统信息" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Info.aspx.cs" Inherits="system_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <table width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：系统管理 >>修改信息</td></tr>
		<tr><td class="doaction"></td></tr>
        <tr><td>
            <%= GetSystemInfo()%>
        </td></tr>  
        <tr><td>
        <input id="btnLogin" style="width:80px;height:33px;margin:10px 0 15px 125px;" class="sign-button" type="submit" value="提交" />
        </td></tr>   
    </table>
</asp:Content>

