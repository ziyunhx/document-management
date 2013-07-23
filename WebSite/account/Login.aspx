<%@ Page Title="登录" Language="C#" MasterPageFile="~/Account.master" AutoEventWireup="true"
    CodeFile="Login.aspx.cs" Inherits="Account_Login" %>

<asp:Content ID="ContentPH_head" runat="server" ContentPlaceHolderID="ContentPH_head">
</asp:Content>
<asp:Content ID="ContentPH_main" runat="server" ContentPlaceHolderID="ContentPH_main">
	<div class="separator_2"></div>
    <h2>登录说明:</h2>
	<p style="color:#a1a1a1;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;本系统为内部系统，不对外开放注册，如果您是被授权的用户，请使用用户名和密码登录；如果您希望被授权，请联系系统管理员！</p>
	<div class="separator_2"></div>
	<div class="block_contact_form">
        <asp:label id="lbl_Message" runat="server" ForeColor="Red" Font-Size="14pt"></asp:label>

        <div class="email input-wrapper">
            <p style="font-size:1em;font-weight:bold;color:#a1a1a1;">用户名：
            <asp:textbox id="txtUserId" runat="server"></asp:textbox></p>
        </div>
        <div class="input-wrapper">
            <p style="font-size:1em;font-weight:bold;color:#a1a1a1;">密&nbsp;&nbsp;&nbsp;&nbsp;码：
            <asp:textbox id="txtPwd" runat="server" TextMode="Password"></asp:textbox></p>
        </div>
        <div class="failure" id="summary" style="display: none; "><ul></ul></div><div class="button-wrapper command">
            <input id="btnLogin" style="width:244px;height:33px;" class="sign-button" onclick="return checkLogin();" type="submit" value="登录" runat="server" onserverclick="btnLogin_ServerClick" />
        </div>

        <div class="signin-misc-wrapper" style="display:none;">
            <p class="remember-me" style="float:left;">
            <input type="checkbox" name="rememberme"  />&nbsp;&nbsp;记住我
            <a class="reset-password" target="_blank" style="margin-left:103px;" href="">忘记密码？</a>
            </p>
        </div>
                
		<div class="clear"></div>
	</div>

	<div class="block_location">
		<h2 style="color:#b1b1b1;">联系管理员</h2>
		<p>电&nbsp;&nbsp;&nbsp;&nbsp;话： <asp:Label ID="lab_phone" runat="server" Text=""></asp:Label><br />
        E-mail ： <a href="mailto:#" runat="server" id="email"><asp:Label ID="lab_email" runat="server" Text=""></asp:Label></a></p>
        <p>网&nbsp;&nbsp;&nbsp;&nbsp;站： <a href="#" runat="server" id="website"><asp:Label ID="lab_website" runat="server" Text=""></asp:Label></a><br />
        地&nbsp;&nbsp;&nbsp;&nbsp;址： <asp:Label ID="lab_address" runat="server" Text=""></asp:Label></p>
		<p>说&nbsp;&nbsp;&nbsp;&nbsp;明： <asp:Label ID="lab_remark" runat="server" Text=""></asp:Label></p>
        <br />
	</div>

   <script type="text/javascript">
       function checkLogin() {
           if (trim(document.getElementById("ContentPH_main_txtUserId").value) == "") {
               alert('请输入用户名 !');
               document.getElementById("ContentPH_main_txtUserId").focus();
               return false;
           }

           if (trim(document.getElementById("ContentPH_main_txtPwd").value) == "") {
               alert('请输入密码 !');
               document.getElementById("ContentPH_main_txtPwd").focus();
               return false;
           }
       }

       //去除字符串两头的空格
       function trim(s) {
           if (s == null) {
               return s;
           }
           var i;
           var beginIndex = 0;
           var endIndex = s.length - 1;
           for (i = 0; i < s.length; i++) {
               if (s.charAt(i) == ' ' || s.charAt(i) == '　') {
                   beginIndex++;
               }
               else {
                   break;
               }
           }
           for (i = s.length - 1; i >= 0; i--) {
               if (s.charAt(i) == ' ' || s.charAt(i) == '　') {
                   endIndex--;
               }
               else {
                   break;
               }
           }

           if (endIndex < beginIndex) {
               return "";
           }
           return s.substring(beginIndex, endIndex + 1);
       }
	</script>

</asp:Content>