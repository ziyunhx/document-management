<%@ Page Title="编辑用户" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeFile="EditUser.aspx.cs" Inherits="account_EditUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" runat="Server">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="100%" height="26" style="padding-left: 10px;" class="leftmenu">
                当前位置：用户管理 >> 编辑用户
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
                            <span>用户名：</span>
                        </td>
                        <td>
                            <input type="text" id="txtUserID" runat="server" readonly="readonly" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>用户邮箱：</span>
                        </td>
                        <td>
                            <input type="text" id="txtEmail" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>用户密码：</span>
                        </td>
                        <td>
                            <input type="password" id="txtPassword" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>确认密码：</span>
                        </td>
                        <td>
                            <input type="password" id="Password1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>所属角色：</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="selIsState" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>所属岗位：</span>
                        </td>
                        <td>
                            <asp:ListBox ID="List_Organ" runat="server" SelectionMode="Multiple"></asp:ListBox>按住ctrl可多选
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>真实姓名：</span>
                        </td>
                        <td>
                            <input type="text" id="txtUserName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>联系电话：</span>
                        </td>
                        <td>
                            <input type="text" id="txtTelephone" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            <span>QQ：</span>
                        </td>
                        <td>
                            <input type="text" id="TextBox1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="center">
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
