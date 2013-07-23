<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="DocumentSH.aspx.cs" Inherits="document_DocumentSH" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
   <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="100%" height="26" style="padding-left: 10px;" class="leftmenu">
                当前位置：公文管理 >> 公文审核
            </td>
        </tr>
        <tr>
            <td class="doaction">
            </td>
        </tr>
        <tr>
            <td width="100%" valign="top" align="left">
                <table id="tongyong" style="display: block;" width="100%" cellpadding="3" cellspacing="0">
                    <tr>
                        <td class="infoname">
                            公文名称：
                        </td>
                        <td class="abcright">
                            <asp:Label ID="txtName" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            公文说明：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <textarea id="txtReMark" runat="server" rows="5" cols="5" disabled="disabled"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            审批结果：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <asp:DropDownList ID="DropDown_sp" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            审批意见：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <textarea id="txtSP" runat="server" rows="5" cols="5"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <input id="btnEdit" type="button" value="修 改" style="width: 80px; height: 33px; margin: 10px 20px 15px 85px;
                                float: left" class="sign-button" runat="server" onserverclick="btnEdit_ServerClick" />
                            <input id="btnCancel" type="button" value="返 回" style="width: 80px; height: 33px;
                                float: left;" class="sign-button" onclick="history.go(-1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

