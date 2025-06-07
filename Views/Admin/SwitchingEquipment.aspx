<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SwitchingEquipment.aspx.cs" Inherits="equipments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:SqlDataSource ID="equipmentes" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT eq.id, eq.id_nodes, eq.id_model, eq.id_owner, n.name AS node, m.name AS model, ow.name AS [owner], eq.serial_number, eq.inventory_number, eq.mac, eq.ip
                       FROM tc_switching_equipment AS eq
                       INNER JOIN tc_model_equipment AS m ON m.id = eq.id_model
                       INNER JOIN tc_nodes_communication AS n ON n.id = eq.id_nodes
                       INNER JOIN tc_switching_manufacturer AS ow ON ow.id = eq.id_owner
                       WHERE 
                       (@node = '0' OR n.name = @node)
                       AND (@model = '0' OR m.name = @model)
                       AND (@owner = '0' OR ow.name = @owner)
                       AND (eq.serial_number LIKE CONCAT('%', LTRIM(@serial), '%'))
                       AND (eq.inventory_number LIKE CONCAT('%', LTRIM(@inventory), '%'))
                       AND (eq.mac LIKE CONCAT('%', LTRIM(@mac), '%'))
                       AND (eq.ip LIKE CONCAT('%', LTRIM(@ip), '%'))
                       GROUP BY eq.id, eq.id_nodes, eq.id_model, eq.id_owner, n.name, m.name, ow.name, eq.serial_number, eq.inventory_number, eq.mac, eq.ip"
       
         UpdateCommand="UPDATE tc_switching_equipment 
                       SET id_model = @id_model, 
                           id_nodes = @id_nodes, 
                           id_owner = @id_owner, 
                           serial_number = @serial_number, 
                           inventory_number = @inventory_number, 
                           mac = @mac, 
                           ip = @ip 
                           WHERE id = @id">
        <SelectParameters>
            <asp:ControlParameter Name="node"  ControlID="DropDownList4" PropertyName="SelectedValue" DefaultValue="0" />
            <asp:ControlParameter Name="model"  ControlID="DropDownList5" PropertyName="SelectedValue" DefaultValue="0" />
            <asp:ControlParameter Name="owner"  ControlID="DropDownList6" PropertyName="SelectedValue" DefaultValue="0" />
            <asp:ControlParameter Name="serial" ControlID="TextBox9" PropertyName="Text" DefaultValue=" " />
            <asp:ControlParameter Name="inventory"  ControlID="TextBox10" PropertyName="Text" DefaultValue=" " />
            <asp:ControlParameter Name="mac"  ControlID="TextBox11" PropertyName="Text" DefaultValue=" " />
            <asp:ControlParameter Name="ip"  ControlID="TextBox12" PropertyName="Text" DefaultValue=" " />
        </SelectParameters>

        <UpdateParameters>
            <asp:Parameter Name="id_model" Type="Int32" />
            <asp:Parameter Name="id_nodes" Type="Int32" />
            <asp:Parameter Name="id_owner" Type="Int32" />
            <asp:Parameter Name="serial_number" Type="String" />
            <asp:Parameter Name="inventory_number" Type="String" />
            <asp:Parameter Name="mac" Type="String" />
            <asp:Parameter Name="ip" Type="String" />
            <asp:Parameter Name="id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>


    <asp:SqlDataSource ID="nodes" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT * FROM tc_nodes_communication"></asp:SqlDataSource>

    <asp:SqlDataSource ID="models" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT * FROM tc_model_equipment"></asp:SqlDataSource>

    <asp:SqlDataSource ID="owners" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
        SelectCommand="SELECT * FROM tc_switching_manufacturer"></asp:SqlDataSource>

    <div class="row" style="padding: 0; margin: 0 auto; width: 75%;">
        <h1>Коммутационное оборудование</h1>

        <%--Фильтры--%>
        <div class="filter-panel" style="background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1); margin-bottom: 20px;">
            <h3 style="margin: 0; font-size: 1.5em; color: #333;">Фильтры</h3>
            <div class="filter-container" style="margin-top: 15px; display: flex; flex-wrap: nowrap; gap: 20px;">

                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="LabelFilterNode" runat="server" Text="Узел:" />
                    <asp:DropDownList ID="DropDownList4" runat="server" DataSourceID="nodes" DataTextField="name" DataValueField="name" AutoPostBack="true" AppendDataBoundItems="true"
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                        <asp:ListItem Text="Показать всё" Value="0" />
                    </asp:DropDownList>
                </div>

                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="Label11" runat="server" Text="Модель оборудования:" />
                    <asp:DropDownList ID="DropDownList5" runat="server" DataSourceID="models" DataTextField="name" DataValueField="name" AutoPostBack="true" AppendDataBoundItems="true"
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                        <asp:ListItem Text="Показать всё" Value="0" />
                    </asp:DropDownList>
                </div>

                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="Label12" runat="server" Text="Владелец:" />
                    <asp:DropDownList ID="DropDownList6" runat="server" DataSourceID="owners" DataTextField="name" DataValueField="name" AutoPostBack="true" AppendDataBoundItems="true"
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;">
                        <asp:ListItem Text="Показать всё" Value="0" />
                    </asp:DropDownList>
                </div>
            </div>

            <div class="filter-container" style="margin-top: 15px; display: flex; flex-wrap: nowrap; gap: 20px;">
                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="Label13" runat="server" Text="Серийный номер:" />
                    <asp:TextBox ID="TextBox9" runat="server" AutoPostBack="true" Text=""
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;" />
                </div>

                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="Label14" runat="server" Text="Инвентарный номер:" />
                    <asp:TextBox ID="TextBox10" runat="server" AutoPostBack="true" 
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;" />
                </div>

                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="Label15" runat="server" Text="MAC-адрес:" />
                    <asp:TextBox ID="TextBox11" runat="server" AutoPostBack="true" 
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;" />
                </div>

                <div class="filter-item" style="flex: 1;">
                    <asp:Label ID="Label16" runat="server" Text="IP-адрес:" />
                    <asp:TextBox ID="TextBox12" runat="server" AutoPostBack="true"
                        Style="width: 100%; padding: 10px; border-radius: 4px; border: 1px solid #ccc;" />
                </div>
            </div>
        </div>
        <%--Конец Фильтров--%>

        <div>
            <asp:LinkButton ID="LinkButton1" runat="server" data-toggle="modal" data-target="#eqNew" CssClass="btn btn-success" Style="float: right;">
                Добавить оборудование
            </asp:LinkButton><br />
            <br />
            <asp:GridView ID="GridView1" runat="server" DataSourceID="equipmentes" DataKeyNames="id" AutoGenerateColumns="False" CssClass="table table-bordered"
                AllowPaging="True" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" PageSize="50" Width="100%" HorizontalAlign="Center"
                OnPageIndexChanging="GridView1_PageIndexChanging" OnRowUpdating="GridView1_RowUpdating">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="" Visible="false" />
                    <asp:TemplateField HeaderText="Узел" SortExpression="node">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="nodes" DataTextField="name" DataValueField="id"
                                Width="100%" CssClass="form-control" SelectValue='<%# Eval("id_nodes") %>'>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("node") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Модель" SortExpression="model">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="models" DataTextField="name" DataValueField="id"
                                Width="100%" CssClass="form-control" SelectValue='<%# Eval("id_model") %>'>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("model") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Владелец" SortExpression="owner">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="owners" DataTextField="name" DataValueField="id"
                                Width="100%" CssClass="form-control" SelectValue='<%# Eval("id_owner") %>'>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("owner") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Серийный номер" SortExpression="serial_number">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("serial_number") %>' Width="100%" CssClass="form-control" />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("serial_number") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Инвентарный номер" SortExpression="inventory_number">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("inventory_number") %>' Width="100%" CssClass="form-control" />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("inventory_number") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="MAC-адрес" SortExpression="mac">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("mac") %>' Width="100%" CssClass="form-control" />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("mac") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IP-адрес" SortExpression="ip">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("ip") %>' Width="100%" CssClass="form-control" />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("ip") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Действия">
                        <ItemTemplate>
                            <asp:Button ID="Button1" runat="server" Text="Ред." CommandName="Edit" CssClass="btn btn-primary" />
                            <asp:Button ID="Button3" runat="server" Text="Удалить" CommandName="Delete" CssClass="btn btn-danger" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Button ID="Button1" runat="server" Text="Сохранить" CommandName="Update" CssClass="btn btn-success" />
                            <asp:Button ID="Button3" runat="server" Text="Отменить" CommandName="Cancel" CssClass="btn btn-success" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
        </div>

        <%--Модальное окно для добавления нового оборудования--%>
        <div id="eqNew" class="modal fade" role="dialog" data-backdrop="false" data-keyboard="false">
            <div class="modal-dialog" style="width: 50%;">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                        <h4 class="modal-title">Добавить оборудование</h4>
                    </div>
                    <div class="modal-body" style="font-size: 12px;">
                        <div class="row form-group">
                            <div class="col-md-4">
                                <asp:Label ID="Label4" runat="server" Text="Узел" />
                                <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="nodes" DataTextField="name" DataValueField="id"
                                    Width="100%" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <asp:Label ID="Label5" runat="server" Text="Модель оборудования" />
                                <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="models" DataTextField="name" DataValueField="id"
                                    Width="100%" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <asp:Label ID="Label8" runat="server" Text="Владелец оборудования" />
                                <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="owners" DataTextField="name" DataValueField="id"
                                    Width="100%" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-6">
                                <asp:Label ID="Label2" runat="server" Text="Серийный номер" />
                                <asp:TextBox ID="TextBox2" runat="server" Width="100%" CssClass="form-control" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label ID="Label3" runat="server" Text="Инвентарный номер" />
                                <asp:TextBox ID="TextBox3" runat="server" Width="100%" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-6">
                                <asp:Label ID="Label9" runat="server" Text="MAC-адрес" />
                                <asp:TextBox ID="TextBox1" runat="server" Width="100%" CssClass="form-control" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label ID="Label10" runat="server" Text="IP-адрес" />
                                <asp:TextBox ID="TextBox8" runat="server" Width="100%" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="Button2" runat="server" Text="Сохранить" CssClass="btn btn-success" OnClick="Button2_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
