<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AddRole.aspx.cs" Inherits="account_AddRole" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
    <script language="JavaScript" src="/scripts/getids.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function postBackByObject() {
            var o = window.event.srcElement;
            if (o.tagName == "INPUT" && o.type == "checkbox") //点击treeview的checkbox是触发  
            {
                var d = o.id; //获得当前checkbox的id;  
                var e = d.replace("CheckBox", "Nodes"); //通过查看脚本信息,获得包含所有子节点div的id  
                var div = window.document.getElementById(e); //获得div对象  
                if (div != null)  //如果不为空则表示,存在自节点  
                {
                    var check = div.getElementsByTagName("INPUT"); //获得div中所有的已input开始的标记  
                    for (i = 0; i < check.length; i++) {
                        if (check[i].type == "checkbox") //如果是checkbox  
                        {
                            check[i].checked = o.checked; //字节点的状态和父节点的状态相同,即达到全选  
                        }
                    }
                }
                else  //点子节点的时候,使父节点的状态改变,即不为全选  
                {
                    var divid = o.parentElement.parentElement.parentElement.parentElement.parentElement; //子节点所在的div 
                    var id = divid.id.replace("Nodes", "CheckBox"); //获得根节点的id  
                    var checkbox = divid.getElementsByTagName("INPUT"); //获取所有子节点数  
                    var s = 0;
                    for (i = 0; i < checkbox.length; i++) {
                        if (checkbox[i].checked)  //判断有多少子节点被选中  
                        {
                            s++;
                        }
                    }
                    if (s <= checkbox.length && s > 0)  //如果全部选中 或者 选择的是另外一个根节点的子节点 ，  
                    {                               //    则开始的根节点的状态仍然为选中状态  
                        window.document.getElementById(id).checked = true;
                    }
                    else {                               //否则为没选中状态  
                        window.document.getElementById(id).checked = false;
                    }
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <table width="100%" border="0" cellpadding="0" cellspacing="0" >
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：用户管理 >>角色编辑</td></tr>
		<tr><td class="doaction" align="left">
		角色名字：<input type="text" id="name" runat="server" style="width: 92px" />
		角色状态:<select id="isState" runat="server"><option value="1">启用</option><option value="0">暂停</option></select>
		<input id="btn_ck" value=" 提  交 " type="button" runat="server" onserverclick="btn_ck_ServerClick"  />
		<%--<span class="action-span"><a href="AdminAuthorize.aspx">返回</a></span>--%>
		</td></tr>
		<tr>
		    <td>
                <asp:TreeView ID="treeV_Authorize" runat="server" ShowCheckBoxes="All" onclick="javascript:return postBackByObject();">
                </asp:TreeView>
            </td>
		</tr>
	</table>
</asp:Content>

