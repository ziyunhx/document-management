<%@ Page Title="主页" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeFile="Index.aspx.cs" Inherits="_Default" %>

<asp:Content ID="ContentPH_head" runat="server" ContentPlaceHolderID="ContentPH_head">
</asp:Content>
<asp:Content ID="ContentPH_main" runat="server" ContentPlaceHolderID="ContentPH_main">
    <h2>
        欢迎使用新点公文管理系统!
    </h2>
    <p>
        您有 <a href="/document/AuditDocument.aspx" title="公文审批"><asp:Label ID="Lab_auditnum" runat="server" Text="0"></asp:Label>条</a>公文等待审批， 
            <a href="/document/PublicDocument.aspx" title="公文发布"><asp:Label ID="Lab_publicnum" runat="server" Text="0"></asp:Label>条</a>公文等待发布。
    </p>
    <p>
        您还可以 <a href="/document/DocumentList.aspx" title="历史公文">查看历史公文</a>。
    </p>
</asp:Content>
