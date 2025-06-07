<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="Report.aspx.cs" Inherits="Kursovaya.Views.Reports.Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <div class="header"><h1>Конструктор отчётов</h1></div>

        <div class="content">
            <div class="controls">
                <div class="form-group">
                    <label for="sourceSelect">Источник данных</label>
                    <select id="sourceSelect" class="form-control"></select>
                </div>

                <div class="form-group">
                    <label>Выберите поля</label>
                    <div id="fieldsContainer" class="checkbox-group">
                        <div class="empty-state">Сначала выберите источник данных</div>
                    </div>
                </div>

                <div class="button-group">
                    <button id="generateBtn"  type="button" class="btn btn-primary" disabled>Сформировать отчёт</button>
                    <button id="exportBtn"    type="button" class="btn btn-secondary" disabled>Экспорт в Excel</button>
                </div>
            </div>

            <div class="report-area">
                <div class="report-header">Предварительный просмотр</div>
                <div class="report-content">
                    <div id="reportResult" class="empty-state">
                        <div style="font-size:3rem">📊</div>
                        Выберите источник и поля
                    </div>
                    <div id="loadingState" class="loading" style="display:none">
                        <div class="spinner"></div>
                        Генерация отчёта…
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%= Scripts.Render("~/bundles/custom").ToHtmlString() %>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            custom.init({
                srcSel: 'sourceSelect',
                fldBox: 'fieldsContainer',
                genBtn: 'generateBtn',
                expBtn: 'exportBtn',
                resultEl: 'reportResult',
                loadEl: 'loadingState'
            });
        });
    </script>

    <link rel="stylesheet" href="../../Content/style.css"> 
</asp:Content>
