<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="OrganList.aspx.cs" Inherits="system_OrganList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPH_head" Runat="Server">
    <script type="text/javascript">
        //批量删除岗位(可以针对批量删除)
        function DeleteForCheckOrgan() {
            var chks = document.forms['form1'];
            for (var i = 0; i < chks.length; i++) {
                if (chks.elements[i].type == "checkbox") {
                    if (chks.elements[i].checked) {
                        var str = PageMethods.btnDeleteOrgan(chks[i].value);
                    }
                }
            }
            alert("批量删除成功！");
            location.reload();
        }

        //删除单条岗位(可以针对批量删除)
        function DeleteOrgan(id) {
            var str = PageMethods.btnDeleteOrgan(id);
            alert("删除成功！");
            location.reload();
        }

        //全选
        function selectAll() {
            obj = document.forms['form1'];
            choseall = document.getElementById("selAll");
            for (var i = 0; i < obj.length; i++) {
                if (obj.elements[i].type == "checkbox") {
                    if (choseall.checked)
                        obj.elements[i].checked = true;
                    else
                        obj.elements[i].checked = false;
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPH_main" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>

    <table width="100%" border="0" cellpadding="0" cellspacing="0" >
		<tr><td width="100%" height="26" style="padding-left:10px;" class="leftmenu">当前位置：系统管理 >> 组织架构管理</td></tr>
		<tr><td class="doaction">
        <span class="action-span"><input type="checkbox" id="selAll" name="selAll" onclick="selectAll();" />全选</span>
		<span class="action-spandel"><a href="#" id="delall" runat="server" onclick="DeleteForCheckOrgan();">删除所选</a></span>
		<span class="action-span"><a id="hrarea" runat="server" href="OrganList.aspx">返回主类</a></span>
		<span class="action-span"><a href="/system/AddOrgan.aspx?pid=<%=pid%>" >新增分类</a></span>
		</td></tr>
		<tr style="text-align:center;"><td style="width:100%;">
            <asp:GridView ID="gvClass" HeaderStyle-BackColor="Coral" runat="server" 
                AutoGenerateColumns="False" DataKeyNames="ID" BorderWidth="1px" 
                BorderStyle="Dotted" Width="100%" CellPadding="3">
                <Columns>
                    <asp:TemplateField HeaderText="选择">
                        <ItemTemplate>
							<input type="checkbox" value='<%#Eval("ID") %>' />
						</ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="岗位名称（点击查看下级）">
                        <ItemTemplate>
                            <a href='OrganList.aspx?pid=<%#Eval("ID") %>&name=<%#Eval("Name").ToString().Replace("&","*") %>'><%# Eval("Name") %></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="上级岗位">
                        <ItemTemplate>
                            <%# ForMatRootName(Eval("PID").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="查看下级 / 操作">
                        <ItemTemplate>
                        <a href='OrganList.aspx?pid=<%#Eval("ID") %>&name=<%#Eval("Name").ToString().Replace("&","*") %>'><img border="0" src="../images/icon_view.gif" alt="查看子类" /></a> &nbsp;
                        <a href="/system/EditOrgan.aspx?id=<%#Eval("ID") %>"><img src="../images/icon_edit.gif" alt="编辑"/></a> &nbsp;
                        <img src="../images/no.gif" alt="删除" style="cursor:pointer;" onclick="javascript:DeleteOrgan('<%#Eval("id").ToString()%>',-1);" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle BackColor="Coral" />
                <PagerSettings PageButtonCount="15" />
            </asp:GridView>
		 </td>
	    </tr>
	</table>
</asp:Content>

