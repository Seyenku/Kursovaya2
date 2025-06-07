<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EquipmentModels.aspx.cs" Inherits="models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:SqlDataSource ID="modeles" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>">
        <SelectParameters>
            <asp:ControlParameter Name="manufacturer" ControlID="DropDownList1" PropertyName="SelectedValue" DefaultValue="0" />
            <asp:ControlParameter Name="namemodel" ControlID="TextBox1" PropertyName="Text" DefaultValue=" " />
            <asp:ControlParameter Name="equipmentType" ControlID="DropDownList2" PropertyName="SelectedValue" DefaultValue="0" />
            <asp:ControlParameter Name="installationType" ControlID="DropDownList3" PropertyName="SelectedValue" DefaultValue="0" />
            <asp:ControlParameter Name="managedType" ControlID="DropDownList4" PropertyName="SelectedValue" DefaultValue="-1 " />
            <asp:ControlParameter Name="consolePort" ControlID="DropDownList5" PropertyName="SelectedValue" DefaultValue="-1" />
            <asp:ControlParameter Name="poeSupport" ControlID="DropDownList6" PropertyName="SelectedValue" DefaultValue="-1" />
            <asp:ControlParameter Name="port" ControlID="TextBox2" PropertyName="Text" DefaultValue=" " />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="ports" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT t.id, CONCAT(p.quantity, ' x ', con.name, ' ',sp.name) AS res FROM tc_model_port AS p
                INNER JOIN tc_port_type AS t ON t.id = p.id_port
                INNER JOIN tc_port_connector_type AS con ON con.id = t.id_type
                INNER JOIN tc_port_speed AS sp ON sp.id = t.id_speed
                WHERE id_model = @model_id">
        <SelectParameters>
            <asp:Parameter Name="model_id" DbType="Int32" DefaultValue="13" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="equipment" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT * FROM tc_equipment_type"></asp:SqlDataSource>

    <asp:SqlDataSource ID="installation" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT * FROM tc_model_type_installation"></asp:SqlDataSource>

    <asp:SqlDataSource ID="manufacturer" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT * FROM tc_equipment_manufacturer"></asp:SqlDataSource>


    <div class="row" style="padding: 0; margin: 0 auto; width: 75%;">
        <h1>Модели оборудования</h1>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updatePanel1">
            <ContentTemplate>
                <!-- Панель группировки -->
                <div class="grouping-panel" style="background-color: #ffffff; padding: 15px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1); margin-bottom: 20px;">
                    <h3 style="margin: 0; font-size: 1.5em; color: #333;">Группировка</h3>
                    <div style="margin-top: 10px; display: flex; flex-direction: row; flex-wrap: wrap; gap: 10px;" class="checkbox-container" id="groupingCheckboxes">
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn1" runat="server" Text="Производитель" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="creator" data-index="0" Style="width: 100%;" />
                        </div>
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn2" runat="server" Text="Название модели" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="name" data-index="1" Style="width: 100%;" />
                        </div>
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn3" runat="server" Text="Тип оборудования" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="typee" data-index="2" Style="width: 100%;" />
                        </div>
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn4" runat="server" Text="Тип установки" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="inst" data-index="3" Style="width: 100%;" />
                        </div>
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn5" runat="server" Text="Управляемый" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="managed" data-index="4" Style="width: 100%;" />
                        </div>
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn6" runat="server" Text="Консольный порт" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="console" data-index="5" Style="width: 100%;" />
                        </div>
                        <div style="flex: 1 1 auto;">
                            <asp:CheckBox ID="chkColumn7" runat="server" Text="Поддержка PoE" AutoPostBack="false" CssClass="group-checkbox js-group-checkbox" data-column="poe" data-index="6" Style="width: 100%;" />
                        </div>
                    </div>
                    <div style="margin-top: 10px; text-align: right;">
                        <button type="button" id="applyGroupingBtn" class="btn btn-primary" style="display: none;">Применить группировку</button>
                    </div>
                </div>

                <style>
                    @media (max-width: 600px) {
                        .checkbox-container {
                            flex-direction: column !important;
                        }
                    }
                    
                    .hidden-column {
                        display: none;
                    }
                    
                    .group-header {
                        background-color: #f5f5f5;
                        font-weight: bold;
                        padding: 8px;
                        cursor: pointer;
                        border-bottom: 1px solid #ddd;
                    }
                    
                    .group-header:hover {
                        background-color: #eaeaea;
                    }
                    
                    .group-content {
                        display: none;
                    }
                    
                    .group-expanded .group-content {
                        display: table-row-group;
                    }
                    
                    .group-collapse-icon {
                        margin-right: 8px;
                    }
                </style>
                <!-- Конец Панели группировки -->

                <!-- Панель фильтров -->
                <div class="filter-panel" style="background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1); margin-bottom: 20px;">
                    <h3 style="margin: 0; font-size: 1.5em; color: #333;">Фильтры</h3>
                    <div class="filter-container" style="margin-top: 15px; display: flex; flex-wrap: wrap; gap: 15px; overflow: visible;">

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="LabelFilterManufacturer" runat="server" Text="Производитель:" />
                            <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="manufacturer" DataTextField="name" DataValueField="name" AutoPostBack="true" AppendDataBoundItems="true"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc; position: relative; z-index: 10;">
                                <asp:ListItem Text="Показать всё" Value="0" />
                            </asp:DropDownList>
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="Label2" runat="server" Text="Название модели:" />
                            <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="true" CssClass="js-filter-input" data-filter-type="text" data-column="name"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;" />
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="LabelFilterEquipment" runat="server" Text="Тип оборудования:" />
                            <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="equipment" DataTextField="name" DataValueField="name" AutoPostBack="true" AppendDataBoundItems="true"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                                <asp:ListItem Text="Показать всё" Value="0" />
                            </asp:DropDownList>
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="LabelFilterInstalation" runat="server" Text="Тип установки:" />
                            <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="installation" DataTextField="name" DataValueField="name" AutoPostBack="true" AppendDataBoundItems="true"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                                <asp:ListItem Text="Показать всё" Value="0" />
                            </asp:DropDownList>
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="LabelFilterManaged" runat="server" Text="Тип управляемости:" />
                            <asp:DropDownList ID="DropDownList4" runat="server" AutoPostBack="true" AppendDataBoundItems="true" CssClass="js-filter-select" data-filter-type="boolean" data-column="managed"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                                <asp:ListItem Text="Показать всё" Value="-1" />
                                <asp:ListItem Text="Да" Value="1" />
                                <asp:ListItem Text="Нет" Value="0" />
                            </asp:DropDownList>
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="LabelFilterPort" runat="server" Text="Консольный порт:" />
                            <asp:DropDownList ID="DropDownList5" runat="server" AutoPostBack="true" AppendDataBoundItems="true" CssClass="js-filter-select" data-filter-type="boolean" data-column="console"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                                <asp:ListItem Text="Показать всё" Value="-1" />
                                <asp:ListItem Text="Да" Value="1" />
                                <asp:ListItem Text="Нет" Value="0" />
                            </asp:DropDownList>
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="LabelFilterPoE" runat="server" Text="Поддержка PoE:" />
                            <asp:DropDownList ID="DropDownList6" runat="server" AutoPostBack="true" AppendDataBoundItems="true" CssClass="js-filter-select" data-filter-type="boolean" data-column="poe"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                                <asp:ListItem Text="Показать всё" Value="-1" />
                                <asp:ListItem Text="Да" Value="1" />
                                <asp:ListItem Text="Нет" Value="0" />
                            </asp:DropDownList>
                        </div>

                        <div class="filter-item" style="flex: 1 1 calc(25% - 15px);">
                            <asp:Label ID="Label3" runat="server" Text="Порты:" />
                            <asp:TextBox ID="TextBox2" runat="server" AutoPostBack="true" CssClass="js-filter-input" data-filter-type="text" data-column="ports"
                                Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;" />
                        </div>
                    </div>
                    <div style="margin-top: 15px; text-align: right;">
                        <button type="button" id="applyFiltersBtn" class="btn btn-primary">Применить фильтры</button>
                        <button type="button" id="resetFiltersBtn" class="btn btn-secondary">Сбросить фильтры</button>
                    </div>
                </div>

                <style>
                    @media (max-width: 600px) {
                        .filter-item {
                            flex: 1 1 100% !important;
                        }
                    }
                </style>
                <!-- Конец Панели фильтров -->

                <div class="col-md-12" style="width: 100%;">
                    <asp:ListView ID="ListView1" runat="server" DataKeyNames="id" DataSourceID="modeles" OnItemDataBound="ListView1_ItemDataBound">
                        <EditItemTemplate>
                            <tr style="">
                                <td>
                                    <asp:Button ID="UpdateButton" runat="server" CommandName="Update" Text="Update" />
                                    <asp:Button ID="CancelButton" runat="server" CommandName="Cancel" Text="Cancel" />
                                </td>
                                <td>
                                    <asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("creator") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="name1TextBox" runat="server" Text='<%# Bind("name") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="typeeTextBox" runat="server" Text='<%# Bind("typee") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="instTextBox" runat="server" Text='<%# Bind("inst") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="managedTextBox" runat="server" Text='<%# Bind("managed") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="consoleTextBox" runat="server" Text='<%# Bind("console") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="poeTextBox" runat="server" Text='<%# Bind("poe") %>' />
                                </td>
                            </tr>
                        </EditItemTemplate>
                        <EmptyDataTemplate>
                            <table runat="server" style="">
                                <tr>
                                    <td>No data was returned.</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                        <InsertItemTemplate>
                            <tr style="">
                                <td>
                                    <asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" />
                                    <asp:Button ID="CancelButton" runat="server" CommandName="Cancel" Text="Clear" />
                                </td>
                                <td>
                                    <asp:TextBox ID="idTextBox" runat="server" Text='<%# Bind("id") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="nameTextBox" runat="server" Text='<%# Bind("creator") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="name1TextBox" runat="server" Text='<%# Bind("name") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="typeeTextBox" runat="server" Text='<%# Bind("typee") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="instTextBox" runat="server" Text='<%# Bind("inst") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="managedTextBox" runat="server" Text='<%# Bind("managed") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="consoleTextBox" runat="server" Text='<%# Bind("console") %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="poeTextBox" runat="server" Text='<%# Bind("poe") %>' />
                                </td>
                            </tr>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <tr class="data-row" 
                                data-creator='<%# Eval("creator") %>' 
                                data-name='<%# Eval("name") %>' 
                                data-typee='<%# Eval("typee") %>' 
                                data-inst='<%# Eval("inst") %>' 
                                data-managed='<%# Eval("managed") %>' 
                                data-console='<%# Eval("console") %>' 
                                data-poe='<%# Eval("poe") %>'>
                                <td>
                                    <asp:Label ID="nameLabel" runat="server" Text='<%# Eval("creator") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="name1Label" runat="server" Text='<%# Eval("name") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="typeeLabel" runat="server" Text='<%# Eval("typee") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="instLabel" runat="server" Text='<%# Eval("inst") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="managedLabel" runat="server" Text='<%# (Eval("managed").ToString() == "0") ? "Нет" : "Да" %>' />
                                </td>
                                <td>
                                    <asp:Label ID="consoleLabel" runat="server" Text='<%# (Eval("console").ToString() == "0") ? "Нет" : "Да" %>' />
                                </td>
                                <td>
                                    <asp:Label ID="poeLabel" runat="server" Text='<%# (Eval("poe").ToString() == "0") ? "Нет" : "Да" %>' />
                                </td>
                                <td class="ports-column">
                                    <asp:ListView ID="ListView2" runat="server" DataKeyNames="id">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("res") %>' /><br />
                                        </ItemTemplate>
                                        <LayoutTemplate>
                                            <table id="itemPlaceholderContainer" runat="server" border="0" style="">
                                                <tr runat="server" style="">
                                                </tr>
                                                <tr id="itemPlaceholder" runat="server">
                                                </tr>
                                            </table>
                                        </LayoutTemplate>
                                    </asp:ListView>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <LayoutTemplate>
                            <table runat="server" style="width: 100%;">
                                <tr runat="server">
                                    <td runat="server">
                                        <table id="itemPlaceholderContainer" runat="server" border="0" style="margin: 0 auto; width: 100%;" class="table table-bordered" data-table-id="modelsTable">
                                            <tr runat="server" style="background-color: #5D7B9D; color: white;">
                                                <th runat="server" class="sortable" data-column="creator">Производитель</th>
                                                <th runat="server" class="sortable" data-column="name">Название модели</th>
                                                <th runat="server" class="sortable" data-column="typee">Тип оборудования</th>
                                                <th runat="server" class="sortable" data-column="inst">Тип установки</th>
                                                <th runat="server" class="sortable" data-column="managed">Управляемый</th>
                                                <th runat="server" class="sortable" data-column="console">Консольный порт</th>
                                                <th runat="server" class="sortable" data-column="poe">Поддержка PoE</th>
                                                <th runat="server" class="sortable" data-column="ports">Порты</th>
                                            </tr>
                                            <tr id="itemPlaceholder" runat="server">
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr runat="server">
                                    <td runat="server" style="">
                                        <asp:DataPager ID="DataPager1" runat="server" PageSize="50">
                                            <Fields>
                                                <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                                <asp:NumericPagerField />
                                                <asp:NextPreviousPagerField ButtonType="Button" ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                            </Fields>
                                        </asp:DataPager>
                                    </td>
                                </tr>
                            </table>
                        </LayoutTemplate>
                        <SelectedItemTemplate>
                            <tr style="">
                                <td>
                                    <asp:Label ID="nameLabel" runat="server" Text='<%# Eval("creator") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="name1Label" runat="server" Text='<%# Eval("name") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="typeeLabel" runat="server" Text='<%# Eval("typee") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="instLabel" runat="server" Text='<%# Eval("inst") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="managedLabel" runat="server" Text='<%# Eval("managed") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="consoleLabel" runat="server" Text='<%# Eval("console") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="poeLabel" runat="server" Text='<%# Eval("poe") %>' />
                                </td>
                                <td>
                                    <asp:ListView ID="ListView2" runat="server" DataKeyNames="id">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("res") %>' /><br />
                                        </ItemTemplate>
                                        <LayoutTemplate>
                                            <table id="itemPlaceholderContainer" runat="server" border="0" style="">
                                                <tr runat="server" style="">
                                                </tr>
                                                <tr id="itemPlaceholder" runat="server">
                                                </tr>
                                            </table>
                                        </LayoutTemplate>
                                    </asp:ListView>
                                </td>
                            </tr>
                        </SelectedItemTemplate>
                    </asp:ListView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="DropDownList1" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="TextBox1" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="DropDownList2" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="DropDownList3" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="DropDownList4" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="DropDownList5" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="DropDownList6" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="TextBox2" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <!-- Подключение jQuery, если он ещё не загружен в мастер-странице -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js" integrity="sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4=" crossorigin="anonymous"></script>
    <!-- Подключение Font Awesome для иконок в группировке -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40/MKBW2W4Rhis/DbILU74C1vSrLJxCq57o941Ym01SwNsOMqvEBFlcgUa6xLiPY/NS5R+E6ztJQ==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    
    <script type="text/javascript">
        // Безопасная инициализация после загрузки страницы
        $(document).ready(function () {
            try {
                // Инициализация
                initializeClientSideGrouping();
                initializeClientSideFiltering();
                
                // Обработчики событий для кнопок
                $('#applyGroupingBtn').on('click', function() {
                    try {
                        applyGrouping();
                    } catch(e) {
                        console.error('Ошибка при группировке:', e);
                        alert('Произошла ошибка при группировке данных.');
                    }
                });
                
                $('#applyFiltersBtn').on('click', function() {
                    try {
                        applyFilters();
                    } catch(e) {
                        console.error('Ошибка при фильтрации:', e);
                        alert('Произошла ошибка при применении фильтров.');
                    }
                });
                
                $('#resetFiltersBtn').on('click', function() {
                    try {
                        resetFilters();
                    } catch(e) {
                        console.error('Ошибка при сбросе фильтров:', e);
                        alert('Произошла ошибка при сбросе фильтров.');
                    }
                });
                
                // При первой загрузке не применяем группировку автоматически
                // Загружаем все данные без группировки и фильтрации
                
            } catch(e) {
                console.error('Ошибка при инициализации:', e);
                alert('Произошла ошибка при инициализации страницы.');
            }
        });
        
        // Функция для группировки данных на стороне клиента
        function initializeClientSideGrouping() {
            try {
                // При изменении чекбоксов немедленно применяем группировку
                $('.js-group-checkbox').on('change', function() {
                    try {
                        applyGrouping();
                    } catch(e) {
                        console.error('Ошибка при автоматической группировке:', e);
                    }
                });
            } catch(e) {
                console.error('Ошибка при инициализации группировки:', e);
            }
        }
        
        // Функция применения группировки
        function applyGrouping() {
            try {
                var table = $('.table-bordered[data-table-id="modelsTable"]');
                if (!table.length) {
                    console.error('Таблица не найдена');
                    return;
                }
                
                var rows = $('.data-row').detach();
                
                // Сначала удаляем все предыдущие группировки
                $('.group-header').remove();
                $('.group-content').remove();
                
                var checkedColumns = [];
                
                // Собираем отмеченные колонки для группировки
                $('.js-group-checkbox:checked').each(function() {
                    checkedColumns.push($(this).data('column'));
                });
                
                if (checkedColumns.length === 0) {
                    // Если ничего не выбрано, просто возвращаем строки обратно в таблицу
                    table.find('#itemPlaceholder').before(rows);
                    return;
                }
                
                // Создаем структуру для группировки
                var groups = {};
                var groupOrder = [];
                
                // Группируем строки
                rows.each(function() {
                    var row = $(this);
                    var groupKey = '';
                    
                    // Строим ключ группировки на основе выбранных колонок
                    checkedColumns.forEach(function(column) {
                        var value = row.data(column);
                        if (value === undefined) {
                            value = '';
                        }
                        groupKey += '|' + value;
                    });
                    
                    if (!groups[groupKey]) {
                        groups[groupKey] = [];
                        groupOrder.push(groupKey);
                    }
                    
                    groups[groupKey].push(row);
                });
                
                // Добавляем группы и строки обратно в таблицу
                groupOrder.forEach(function(groupKey) {
                    var groupRows = groups[groupKey];
                    if (groupRows.length > 0) {
                        var firstRow = groupRows[0];
                        var groupHeaderText = '';
                        
                        // Формируем заголовок группы
                        checkedColumns.forEach(function(column, index) {
                            var value = firstRow.data(column);
                            if (value === undefined) {
                                value = 'Н/Д';
                            }
                            
                            // Преобразуем значения 0/1 в Нет/Да для булевых полей
                            if (column === 'managed' || column === 'console' || column === 'poe') {
                                value = value === '0' ? 'Нет' : 'Да';
                            }
                            
                            if (index > 0) {
                                groupHeaderText += ' | ';
                            }
                            
                            // Добавляем имя колонки и значение
                            var columnNames = {
                                'creator': 'Производитель',
                                'name': 'Название модели',
                                'typee': 'Тип оборудования',
                                'inst': 'Тип установки',
                                'managed': 'Управляемый',
                                'console': 'Консольный порт',
                                'poe': 'Поддержка PoE'
                            };
                            
                            groupHeaderText += columnNames[column] + ': ' + value;
                        });
                        
                        // Создаем заголовок группы
                        var colspan = table.find('th').length;
                        var groupHeaderHtml = '<tr class="group-header">' +
                                           '<td colspan="' + colspan + '">' +
                                           '<i class="fas fa-minus-circle group-collapse-icon"></i>' +
                                           groupHeaderText + ' (' + groupRows.length + ')' +
                                           '</td></tr>';
                        
                        // Добавляем заголовок группы
                        var groupHeader = $(groupHeaderHtml);
                        table.find('#itemPlaceholder').before(groupHeader);
                        
                        // Добавляем контент группы
                        var groupContent = $('<tbody class="group-content"></tbody>');
                        groupRows.forEach(function(row) {
                            groupContent.append(row);
                        });
                        
                        // Добавляем содержимое группы и устанавливаем начальное состояние - развернутое
                        table.find('#itemPlaceholder').before(groupContent);
                        groupHeader.addClass('group-expanded');
                        
                        // Обработчик клика для сворачивания/разворачивания группы
                        groupHeader.on('click', function() {
                            $(this).toggleClass('group-expanded');
                            if ($(this).hasClass('group-expanded')) {
                                $(this).find('.group-collapse-icon').removeClass('fa-plus-circle').addClass('fa-minus-circle');
                                groupContent.show();
                            } else {
                                $(this).find('.group-collapse-icon').removeClass('fa-minus-circle').addClass('fa-plus-circle');
                                groupContent.hide();
                            }
                        });
                    }
                });
            } catch(e) {
                console.error('Ошибка при применении группировки:', e);
                throw e;
            }
        }
        
        // Инициализация фильтрации на стороне клиента
        function initializeClientSideFiltering() {
            try {
                // Обработчики для текстовых полей (для живой фильтрации, если нужно)
                $('.js-filter-input').on('keyup', function() {
                    // При изменении фильтров не применяем сразу - ждем нажатия кнопки
                });
                
                // Обработчики для выпадающих списков (для живой фильтрации, если нужно)
                $('.js-filter-select').on('change', function() {
                    // При изменении фильтров не применяем сразу - ждем нажатия кнопки
                });
            } catch(e) {
                console.error('Ошибка при инициализации фильтрации:', e);
            }
        }
        
        // Функция применения фильтров на стороне клиента
        function applyFilters() {
            try {
                // Получаем все строки
                var rows = $('.data-row');
                if (!rows.length) {
                    console.warn('Строки данных не найдены');
                }
                
                // Для каждой строки проверяем все фильтры
                rows.each(function() {
                    var row = $(this);
                    var shouldShow = true;
                    
                    // Проверяем текстовые фильтры
                    $('.js-filter-input').each(function() {
                        var filter = $(this);
                        var column = filter.data('column');
                        var value = filter.val().toLowerCase();
                        
                        if (value !== '') {
                            var cellValue;
                            
                            // Особая обработка для колонки портов, которая требует проверки содержимого
                            if (column === 'ports') {
                                cellValue = row.find('.ports-column').text().toLowerCase();
                            } else {
                                var dataValue = row.data(column);
                                cellValue = dataValue !== undefined ? dataValue.toString().toLowerCase() : '';
                            }
                            
                            if (cellValue.indexOf(value) === -1) {
                                shouldShow = false;
                            }
                        }
                    });
                    
                    // Проверяем выпадающие списки
                    $('.js-filter-select').each(function() {
                        var filter = $(this);
                        var column = filter.data('column');
                        var value = filter.val();
                        
                        // Проверяем только если выбрано не "Показать всё"
                        if (value !== '-1') {
                            var dataValue = row.data(column);
                            var cellValue = dataValue !== undefined ? dataValue.toString() : '';
                            
                            if (cellValue !== value) {
                                shouldShow = false;
                            }
                        }
                    });
                    
                    // Обработка стандартных ASP.NET DropDownList
                    $('select[id$="DropDownList1"], select[id$="DropDownList2"], select[id$="DropDownList3"]').each(function() {
                        var dropdown = $(this);
                        var value = dropdown.val();
                        
                        // Проверяем только если выбрано не "Показать всё"
                        if (value !== '0') {
                            var column;
                            var id = dropdown.attr('id');
                            
                            // Определяем, какая колонка соответствует этому DropDownList
                            if (id.includes('DropDownList1')) {
                                column = 'creator'; // Производитель
                            } else if (id.includes('DropDownList2')) {
                                column = 'typee'; // Тип оборудования
                            } else if (id.includes('DropDownList3')) {
                                column = 'inst'; // Тип установки
                            }
                            
                            if (column) {
                                var dataValue = row.data(column);
                                var cellValue = dataValue !== undefined ? dataValue.toString() : '';
                                
                                if (cellValue !== value) {
                                    shouldShow = false;
                                }
                            }
                        }
                    });
                    
                    // Показываем или скрываем строку
                    if (shouldShow) {
                        row.show();
                    } else {
                        row.hide();
                    }
                });
                
                // После фильтрации перестраиваем группировку
                applyGrouping();
            } catch(e) {
                console.error('Ошибка при применении фильтров:', e);
                throw e;
            }
        }
        
        // Функция сброса фильтров
        function resetFilters() {
            try {
                // Сбрасываем текстовые поля
                $('.js-filter-input').val('');
                
                // Сбрасываем ASP.NET DropDownList (стандартные списки)
                $('#<%=DropDownList1.ClientID%>').val('0');
                $('#<%=DropDownList2.ClientID%>').val('0');
                $('#<%=DropDownList3.ClientID%>').val('0');
                
                // Сбрасываем пользовательские списки с классом js-filter-select
                $('.js-filter-select').each(function() {
                    $(this).val('-1');
                });
                
                // Установка конкретных значений для DropDownList 4-6
                $('#<%=DropDownList4.ClientID%>').val('-1');
                $('#<%=DropDownList5.ClientID%>').val('-1');
                $('#<%=DropDownList6.ClientID%>').val('-1');
                
                // Текстовые поля с конкретными ID
                $('#<%=TextBox1.ClientID%>').val('');
                $('#<%=TextBox2.ClientID%>').val('');
                
                // Показываем все строки
                $('.data-row').show();
                
                // Удаляем все группировки (если были)
                $('.group-header').remove();
                $('.group-content').remove();
                
                // Восстанавливаем оригинальную структуру таблицы
                var table = $('.table-bordered[data-table-id="modelsTable"]');
                var rows = $('.data-row').detach();
                table.find('#itemPlaceholder').before(rows);
            } catch(e) {
                console.error('Ошибка при сбросе фильтров:', e);
                throw e;
            }
        }
    </script>
</asp:Content>
