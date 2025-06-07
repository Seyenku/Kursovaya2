<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CommunicationNodes.aspx.cs" Inherits="_Default" EnableSessionState="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
        <div class="container-fluid">
        <div class="row">
            <div class="col-12">
                <h1>Коммуникационные узлы</h1>
            </div>
        </div>

        <!-- 1. Древовидная структура (сворачиваемая) -->
        <div class="row">
            <div class="col-12">
                <div class="card mb-3">
                    <div class="card-header">
                        <h3 class="mb-0">
                            <button id="treeToggleBtn" 
                                    class="btn btn-link btn-tree text-left w-100 text-decoration-none p-0" 
                                    type="button">
                                <i class="fa fa-chevron-right mr-2 collapse-icon"></i>
                                Древовидная структура узлов
                            </button>
                        </h3>
                    </div>
                    <div id="treeCollapse">
                        <div class="card-body">
                            <div class="tree-search mb-3">
                                <input type="text" id="treeSearchInput" placeholder="Поиск по дереву..." class="form-control" />
                            </div>
                            <div id="nodesTree" class="tree-container" style="min-height: 300px; overflow: auto; padding: 10px; border: 1px solid #ddd; border-radius: 4px;"></div>
                            <asp:HiddenField ID="TreeDataHidden" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 2. Панель группировки -->
        <div class="row">
            <div class="col-12">
                <div class="card mb-3">
                    <div class="card-header">
                        <h3 class="mb-0">Группировка</h3>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-2">
                                <asp:CheckBox ID="GroupByBuilding" runat="server" Text="Корпус" AutoPostBack="false" CssClass="group-checkbox" />
                            </div>
                            <div class="col-md-2">
                                <asp:CheckBox ID="GroupByName" runat="server" Text="Название узла" AutoPostBack="false" CssClass="group-checkbox" />
                            </div>
                            <div class="col-md-2">
                                <asp:CheckBox ID="GroupByType" runat="server" Text="Тип узла" AutoPostBack="false" CssClass="group-checkbox" />
                            </div>
                            <div class="col-md-3">
                                <asp:CheckBox ID="GroupByDate" runat="server" Text="Дата проверки" AutoPostBack="false" CssClass="group-checkbox" />
                            </div>
                            <div class="col-md-3">
                                <asp:CheckBox ID="GroupByDevices" runat="server" Text="Количество устройств" AutoPostBack="false" CssClass="group-checkbox" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Таблица -->
        <div class="row">
            <div class="col-lg-8">
                <div class="card">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <h3 class="mb-0">Коммуникационные узлы</h3>
                        <asp:LinkButton ID="BtnAddNode" runat="server" data-toggle="modal" data-target="#nodesNew" CssClass="btn btn-success">
                            <i class="fa fa-plus mr-1"></i>Добавить узел
                        </asp:LinkButton>
                    </div>
                    <div class="card-body p-0">
                        <table id="GridCommunication" class="table table-bordered table-hover mb-0">
                            <!-- JS будет динамически добавлять thead и tbody -->
                        </table>
                    </div>
                </div>
            </div>


            <!-- Панель фильтров -->
            <div class="col-lg-4">
                <div class="card">
                    <div class="card-header">
                        <h3 class="mb-0">Фильтры</h3>
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label ID="LabelFilterBuilding" runat="server" Text="Корпус:" CssClass="form-label" />
                            <asp:DropDownList ID="FilterBuilding" runat="server" DataTextField="buildname" DataValueField="buildid"
                                AutoPostBack="false" AppendDataBoundItems="true" CssClass="form-control">
                                <asp:ListItem Text="Показать всё" Value="0" />
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="LabelFilterName" runat="server" Text="Название узла:" CssClass="form-label" />
                            <asp:TextBox ID="FilterName" runat="server" AutoPostBack="false" CssClass="form-control" placeholder="Введите название узла" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="LabelFilterType" runat="server" Text="Тип узла:" CssClass="form-label" />
                            <asp:DropDownList ID="FilterType" runat="server" DataTextField="name" DataValueField="id"
                                AutoPostBack="false" AppendDataBoundItems="true" CssClass="form-control">
                                <asp:ListItem Text="Показать всё" Value="0" />
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="LabelFilterDeviceCnt" runat="server" Text="Количество устройств:" CssClass="form-label" />
                            <asp:TextBox ID="FilterDeviceCnt" runat="server" AutoPostBack="false" CssClass="form-control" placeholder="Точное количество" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="LabelFilterDateFrom" runat="server" Text="Дата проверки от:" CssClass="form-label" />
                            <asp:TextBox ID="FilterDateFrom" runat="server" AutoPostBack="false" TextMode="Date" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="LabelFilterDateTo" runat="server" Text="Дата проверки до:" CssClass="form-label" />
                            <asp:TextBox ID="FilterDateTo" runat="server" AutoPostBack="false" TextMode="Date" CssClass="form-control" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Модальное окно для добавления нового узла -->
    <div id="nodesNew" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Добавить коммуникационный узел</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="FormLabelName" runat="server" Text="Название узла" CssClass="form-label" />
                                <asp:TextBox ID="FormInputName" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="FormLabelDate" runat="server" Text="Дата верификации" CssClass="form-label" />
                                <asp:TextBox ID="FormInputDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="FormLabelBuild" runat="server" Text="Корпус" CssClass="form-label" />
                                <asp:DropDownList ID="FormDropBuilding" runat="server"
                                    DataTextField="buildname" DataValueField="buildid" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="FormLabelType" runat="server" Text="Тип узла" CssClass="form-label" />
                                <asp:DropDownList ID="FormDropType" runat="server"
                                    DataTextField="name" DataValueField="id" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="FormLabelOther" runat="server" Text="Дополнительное оборудование" CssClass="form-label" />
                                <asp:TextBox ID="FormInputOther" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="BtnSaveNode" runat="server" Text="Сохранить" CssClass="btn btn-success" />
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Отмена</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Scripts -->
    <asp:PlaceHolder runat="server">
        
        <%: Styles.Render("~/Content/bootstrap") %>
        <%: Styles.Render("~/Content/jstree") %>
        <%: Styles.Render("~/Content/style") %>

        <%: Scripts.Render("~/bundles/jquery") %>
        <%: Scripts.Render("~/bundles/bootstrap") %>
        <%: Scripts.Render("~/bundles/jstree") %>
        <%= Scripts.Render("~/bundles/custom").ToHtmlString() %>
        <script>
            document.addEventListener('DOMContentLoaded', function () {
                if (typeof custom === 'undefined') {
                    console.error('custom.js не загружен');
                    return;
                }

                custom.init({
                    srcSel: 'sourceSelect',
                    fldBox: 'fieldsContainer',
                    genBtn: 'generateBtn',
                    expBtn: 'exportBtn',
                    resultEl: 'reportResult',
                    loadEl: 'loadingState',

                    gridId: 'GridCommunication',
                    treeData: '<%= TreeDataHidden.ClientID %>',

                    filterIds: {
                        building: '<%= FilterBuilding.ClientID %>',
                        name: '<%= FilterName.ClientID %>',
                        type: '<%= FilterType.ClientID %>',
                        device: '<%= FilterDeviceCnt.ClientID %>',
                        dateFrom: '<%= FilterDateFrom.ClientID %>',
                        dateTo: '<%= FilterDateTo.ClientID %>'
                    },
                    chkIds: {
                        col1: '<%= GroupByBuilding.ClientID %>',
                        col2: '<%= GroupByName.ClientID %>',
                        col3: '<%= GroupByType.ClientID %>',
                        col4: '<%= GroupByDate.ClientID %>',
                        col5: '<%= GroupByDevices.ClientID %>'
                    }
                });
            });

            $('#treeToggleBtn').on('click', function () {
                $('#treeCollapse').collapse('toggle');
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>