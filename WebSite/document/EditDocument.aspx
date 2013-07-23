<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="EditDocument.aspx.cs" Inherits="document_EditDocument" %>

<%@ Register assembly="Brettle.Web.NeatUpload" namespace="Brettle.Web.NeatUpload" tagprefix="Upload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
   <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="100%" height="26" style="padding-left: 10px;" class="leftmenu">
                当前位置：公文管理 >> 编辑公文
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
                            <input type="text" id="txtName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            选择公文：
                        </td>
                        <td id="uploadfiles" class="abcright" style="height: 31px;display:none;" runat="server">
                            <Upload:InputFile ID="UploadFile" runat="server" />
                            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="上传" />
                            <Upload:ProgressBar ID="ProgressBar1" runat="server" />
                        </td>
                        <td id="uploadfileok" class="abcright" style="height: 31px;color:Green;" runat="server">
                            <input id="uploadurl" type="text" runat="server" />
                            <asp:Button ID="Button2" runat="server" Text="删除并重传" onclick="Button2_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            公文说明：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <textarea id="txtReMark" runat="server" rows="5" cols="5"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            工作流：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <asp:DropDownList ID="selWorkFlow" runat="server">
                           </asp:DropDownList>
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

