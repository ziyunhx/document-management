<%@ Page Title="添加用户" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeFile="AddUser.aspx.cs" Inherits="account_AddUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" runat="Server">
    <table width="100%" border="0" cellpadding="3" cellspacing="0">
        <tr>
            <td width="100%" height="26" style="padding-left: 10px;" class="leftmenu">
                当前位置：用户管理 >> 添加用户
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
                            用户名：
                        </td>
                        <td class="abcright">
                            <input type="text" id="txtUserID" runat="server" onblur="checkUserID();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            用户邮箱：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <input type="text" id="txtEmail" runat="server" onblur="checkEmail();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            用户密码：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <input type="password" id="txtPassword" runat="server" onblur="checkPassword();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            确认密码：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <input type="password" id="Text4" runat="server" onblur="checkturepassword();" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>所属角色：</span>
                        </td>
                        <td>
                           <asp:DropDownList ID="selIsState" runat="server">
                           </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>所属岗位：</span>
                        </td>
                        <td>
                            <asp:ListBox ID="List_Organ" runat="server" SelectionMode="Multiple"></asp:ListBox>按住ctrl可多选
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            真实姓名：
                        </td>
                        <td class="abcright">
                            <input type="text" id="txtUserName" runat="server" onblur="checkUserName();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            联系电话：
                        </td>
                        <td class="abcright">
                            <asp:TextBox ID="txtTelephone" runat="server" Columns="50" Rows="3" MaxLength="500"
                                onblur="checkTel();"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            QQ：
                        </td>
                        <td class="abcright" style="height: 31px">
                            <asp:TextBox ID="TextBox1" runat="server" Columns="50" MaxLength="500" Rows="3" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <input id="btnAdd" type="submit" value="增加" style="width: 80px; height: 33px; margin: 10px 20px 15px 85px;
                                float: left" class="sign-button" runat="server" onserverclick="btnAdd_ServerClick" />
                            <input id="btnCancel" type="button" value="返回" style="width: 80px; height: 33px;
                                float: left;" class="sign-button" onclick="history.go(-1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
