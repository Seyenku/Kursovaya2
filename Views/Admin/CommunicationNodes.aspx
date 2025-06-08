<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <title>Коммуникационные узлы</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/font-awesome@4.7.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="/Content/jstree/style.min.css">
    <link rel="stylesheet" href="/Content/style.css">
</head>
<body>

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <h1>Коммуникационные узлы</h1>
        </div>
    </div>

    <!-- 1. Древовидная структура -->
    <div class="row">
        <div class="col-12">
            <div class="card mb-3">
                <div class="card-header">
                    <h3 class="mb-0">
                        <button id="treeToggleBtn" type="button" 
                            class="btn btn-link btn-tree text-left w-100 p-0"
                            data-toggle="collapse" data-target="#treeCollapse">
                            <i class="fa fa-chevron-right mr-2 collapse-icon"></i>
                            Древовидная структура узлов
                        </button>
                    </h3>
                </div>
                <div id="treeCollapse" class="collapse show">
                    <div class="card-body">
                        <div class="tree-search mb-3">
                            <input type="text" id="treeSearchInput" placeholder="Поиск по дереву..." class="form-control">
                        </div>
                        <div id="nodesTree" class="tree-container" style="min-height: 300px; overflow: auto; padding: 10px; border: 1px solid #ddd; border-radius: 4px;"></div>
                        <input type="hidden" id="TreeDataHidden">
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
                            <div class="form-check">
                                <input type="checkbox" id="GroupByBuilding" class="form-check-input group-checkbox">
                                <label for="GroupByBuilding" class="form-check-label">Корпус</label>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-check">
                                <input type="checkbox" id="GroupByName" class="form-check-input group-checkbox">
                                <label for="GroupByName" class="form-check-label">Название узла</label>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-check">
                                <input type="checkbox" id="GroupByType" class="form-check-input group-checkbox">
                                <label for="GroupByType" class="form-check-label">Тип узла</label>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-check">
                                <input type="checkbox" id="GroupByDate" class="form-check-input group-checkbox">
                                <label for="GroupByDate" class="form-check-label">Дата проверки</label>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-check">
                                <input type="checkbox" id="GroupByDevices" class="form-check-input group-checkbox">
                                <label for="GroupByDevices" class="form-check-label">Количество устройств</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Таблица и фильтры -->
    <div class="row">
        <!-- Таблица -->
        <div class="col-lg-8">
            <div class="card table">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h3 class="mb-0">Коммуникационные узлы</h3>
                    <button id="BtnAddNode" class="btn btn-success" data-toggle="modal" data-target="#nodesNew">
                        <i class="fa fa-plus mr-1"></i>Добавить узел
                    </button>
                </div>
                <div class="card-body p-0">
                    <table id="GridCommunication" class="table table-bordered table-hover mb-0">
                        <thead>
                            <tr>
                                <th>Корпус</th>
                                <th>Название</th>
                                <th>Тип</th>
                                <th>Доп.</th>
                                <th>Дата</th>
                                <th>Кол-во</th>
                                <th>Действия</th>
                            </tr>
                        </thead>
                        <tbody><!-- Содержимое таблицы будет заполнено JavaScript --></tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Фильтры -->
        <div class="col-lg-4">
            <div class="card">
                <div class="card-header">
                    <h3 class="mb-0">Фильтры</h3>
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <label for="FilterBuilding" class="form-label">Корпус:</label>
                        <select id="FilterBuilding" class="form-control">
                            <option value="0">Показать всё</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="FilterName" class="form-label">Название узла:</label>
                        <input type="text" id="FilterName" class="form-control" placeholder="Введите название узла">
                    </div>
                    <div class="form-group">
                        <label for="FilterType" class="form-label">Тип узла:</label>
                        <select id="FilterType" class="form-control">
                            <option value="0">Показать всё</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="FilterDeviceCnt" class="form-label">Количество устройств:</label>
                        <input type="text" id="FilterDeviceCnt" class="form-control" placeholder="Точное количество">
                    </div>
                    <div class="form-group">
                        <label for="FilterDateFrom" class="form-label">Дата проверки от:</label>
                        <input type="date" id="FilterDateFrom" class="form-control">
                    </div>
                    <div class="form-group">
                        <label for="FilterDateTo" class="form-label">Дата проверки до:</label>
                        <input type="date" id="FilterDateTo" class="form-control">
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
                            <label for="FormInputName" class="form-label">Название узла</label>
                            <input type="text" id="FormInputName" class="form-control">
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="FormInputDate" class="form-label">Дата верификации</label>
                            <input type="date" id="FormInputDate" class="form-control">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="FormDropBuilding" class="form-label">Корпус</label>
                            <select id="FormDropBuilding" class="form-control"></select>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="FormDropType" class="form-label">Тип узла</label>
                            <select id="FormDropType" class="form-control"></select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group">
                            <label for="FormInputOther" class="form-label">Дополнительное оборудование</label>
                            <textarea id="FormInputOther" class="form-control" rows="4"></textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button id="BtnSaveNode" class="btn btn-success">Сохранить</button>
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Отмена</button>
            </div>
        </div>
    </div>
</div>

<!-- Scripts -->
<!-- Базовые библиотеки -->
<script src="../../Scripts/jquery-3.7.0.min.js"></script>
<script src="../../Scripts/bootstrap.min.js"></script>
<script src="../../Scripts/jstree.min.js"></script>
<!-- Модули custom -->
<script src="../../Scripts/custom/util.js"></script>
<script src="../../Scripts/custom/api.js"></script>
<script src="../../Scripts/custom/tree.js"></script>
<script src="../../Scripts/custom/filters.js"></script>
<script src="../../Scripts/custom/grid.js"></script>
<script src="../../Scripts/custom/modal.js"></script>
<script src="../../Scripts/custom/app.js"></script>

<script>
    $(function () {
        CN_App.start({
            gridId: 'GridCommunication',
            filterIds: {
                building: 'FilterBuilding', name: 'FilterName', type: 'FilterType',
                device: 'FilterDeviceCnt', dateFrom: 'FilterDateFrom', dateTo: 'FilterDateTo'
            }
        });
    });
</script>

</body>
</html>