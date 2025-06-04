<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="Kursovaya.Views.Reports.Report" %>
<%@ Import Namespace="Kursovaya.Models" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Конструктор отчетов</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Конструктор отчетов</h2>

            <asp:Label runat="server" Text="Источник данных: " AssociatedControlID="ddlSource" />
            <asp:DropDownList ID="ddlSource" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSource_SelectedIndexChanged" />

            <asp:Label runat="server" Text="Выберите поля:" AssociatedControlID="cblFields" />
            <asp:CheckBoxList ID="cblFields" runat="server" />

            <asp:Button ID="btnGenerate" runat="server" Text="Сформировать отчет" OnClick="btnGenerate_Click" />
            <asp:Button ID="btnExport" runat="server" Text="Экспорт в Excel" OnClick="btnExport_Click" />

            <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="true" />
        </div>
    </form>
</body>
</html>
