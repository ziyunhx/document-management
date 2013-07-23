<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AuthorizeWorkFlow.aspx.cs" Inherits="workflow_AuthorizeWorkFlow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
<script type="text/javascript">
    function getoption() {
        var controls = document.getElementsByName("contopt");
        var str = "";
        for (var i = 0; i < controls.length; i++) {
            var theoption = controls[i];

            var New=document.getElementsByName("myrad"+i.toString());
            var strNew;
            for (var k = 0; k < New.length; k++) {
                if (New.item(k).checked) {
                    strNew = New.item(k).getAttribute("value");
                }
            }
            for (var j = 0; j < theoption.length; j++) {
                if (theoption[j].selected == true) {
                    str += theoption[j].value + ",";
                }
            }
            str += strNew + ";";
        }
        var invalue = document.getElementById("options");
        invalue.value = str;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <table width="100%" border="0" cellpadding="3" cellspacing="0">
        <tr>
            <td width="100%" height="26" style="padding-left: 10px;" class="leftmenu">
                当前位置：工作流管理 >> 工作流授权
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
                        <td colspan="2">
                            创建公文：
                            （选择具有使用该流程创建公文的角色！）
                        </td>
                    </tr>
                    <tr style="padding-bottom:25px;">
                        <td colspan="2">
                            <asp:ListBox ID="List_ADDOrgan" runat="server" SelectionMode="Multiple"></asp:ListBox>按住ctrl可多选
                        </td>                        
                    </tr>
                    <tr>
                        <td class="infoname">
                            公文流转：
                        </td>
                        <td>
                            （选择具有操作工作流中活动的角色！）
                            <input id="options" name="options" type="text" style="display:none;" />
                        </td>
                    </tr>
                    <tr  style="padding-bottom:25px;">
                        <td colspan="2">
                        <table>
                            <%= Initsh()%> 
                        </table>
                            
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            发布公文：
                        </td>
                        <td>
                            （选择具有发布流转完成公文的角色！）
                        </td>
                    </tr>
                    <tr  style="padding-bottom:25px;">
                        <td colspan="2">
                            <asp:ListBox ID="List_PublicOrgan" runat="server" SelectionMode="Multiple"></asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="infoname">
                            查阅公文：
                        </td>
                        <td>
                            （选择具有查阅发布的公文的角色！）
                        </td>
                    </tr>
                    <tr  style="padding-bottom:25px;">
                        <td colspan="2">
                            <asp:ListBox ID="List_Read" runat="server" SelectionMode="Multiple"></asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <input id="btnAdd" type="submit" onclick="getoption()" value="保存" style="width: 80px; height: 33px; margin: 10px 20px 15px 85px;
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

