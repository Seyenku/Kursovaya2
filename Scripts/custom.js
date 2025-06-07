/* --------------------------------------------------------------------
   CommunicationNodes‑ES5.js  (refactored)
   Полностью клиентская фильтрация, загрузка, группировка. Работает в
   .NET Web Forms 4.7.2, C# 7.3 и готово к ASP.NET Core.  jQuery >= 1.9.
-------------------------------------------------------------------- */
var custom = (function () {
    'use strict';

    /* ------------------------------------------------------------
       ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ
    ------------------------------------------------------------ */
    var els = {};      // кэш DOM‑элементов
    var IDs = {};      // clientID‑ы, переданные из init

    /* ------------------------------------------------------------
       ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
    ------------------------------------------------------------ */
    function $id(id) { return document.getElementById(id); }

    // безопасный encode
    function qs(obj) {
        var p = [];
        for (var k in obj) {
            if (obj.hasOwnProperty(k) && obj[k] !== null && obj[k] !== undefined && obj[k] !== '') {
                p.push(encodeURIComponent(k) + '=' + encodeURIComponent(obj[k]));
            }
        }
        return p.join('&');
    }

    function showError(msg) {
        alert('Ошибка: ' + msg);
    }

    /* ------------------------------------------------------------
       ДЕРЕВО УЗЛОВ  (jstree)
    ------------------------------------------------------------ */
    function loadTree() {
        var $tree = $('#nodesTree');
        if (!$tree.length) { return; }

        $.getJSON('/api/nodes/tree')
            .done(function (data) {
                buildTree($tree, data);
            })
            .fail(function (xhr) { showError(xhr.statusText); });
    }

    function buildTree($treeElem, data) {
        $treeElem.empty();
        $treeElem.jstree('destroy');

        if (!$('.highlighted-row-style').length) {
            $('<style class="highlighted-row-style">.highlighted-row{background-color:#ffebb5!important}</style>')
                .appendTo('head');
        }

        $treeElem.jstree({
            core: {
                data: data,
                check_callback: true,
                themes: { responsive: true }
            },
            plugins: ['search', 'state', 'types', 'wholerow'],
            types: { 'default': { icon: 'fa fa-folder' } },
            search: { show_only_matches: true }
        })
            .on('ready.jstree', function () {
                $(this).jstree('close_all');
            })
            .on('select_node.jstree', function (e, d) {
                if (d.node.id.indexOf('node_') === 0) {
                    highlightGridRow(d.node.data.nodeId);
                }
            });

        $treeElem.on('click', '.jstree-anchor', function (e) {
            var tree = $treeElem.jstree(true);
            var node = tree.get_node(this);
            if (tree.is_open(node)) {
                tree.close_node(node);
            } else {
                tree.open_node(node);
            }
        });

        // Поиск по дереву
        var searchInput = $id('treeSearchInput');
        if (searchInput) {
            $(searchInput).on('keyup', function () {
                var val = this.value;
                $treeElem.jstree(true).search(val);
            });
        }
    }

    function highlightGridRow(nodeId) {
        var $rows = $('#' + IDs.gridId + ' tbody tr');
        $rows.removeClass('highlighted-row');

        $rows.each(function () {
            var txt = $(this).find('td:first').text();
            if (txt === String(nodeId)) {
                $(this).addClass('highlighted-row');
                $('html,body').animate({ scrollTop: $(this).offset().top - 100 }, 400);
            }
        });
    }

    /* ------------------------------------------------------------
       ТАБЛИЦА УЗЛОВ + ФИЛЬТРЫ
    ------------------------------------------------------------ */
    // Подгрузка dropdown для корпуса и типа узла
    var mapBuildings = {}, mapTypes = {};
    function loadInitialData() {
        return Promise.all([
            new Promise(function (resolve) {
                $.getJSON('/api/buildings', function (list) {
                    list.forEach(function (b) { mapBuildings[b.id] = b.name; });
                    resolve();
                });
            }),
            new Promise(function (resolve) {
                $.getJSON('/api/nodetypes', function (list) {
                    list.forEach(function (t) { mapTypes[t.id] = t.name; });
                    resolve();
                });
            })
        ]);
    }

    function currentFilter() {
        var buildingEl = $id(IDs.filterIds.building); // Корпус
        var nameEl = $id(IDs.filterIds.name);
        var typeEl = $id(IDs.filterIds.type); // Тип узла
        var deviceEl = $id(IDs.filterIds.device);
        var dateFromEl = $id(IDs.filterIds.dateFrom);
        var dateToEl = $id(IDs.filterIds.dateTo); 

        return {
            BuildingFilter: buildingEl ? buildingEl.value : null,
            NodeNameFilter: nameEl ? nameEl.value.trim() : null,
            TypeFilter: typeEl ? typeEl.value : null,
            DeviceCount: deviceEl ? deviceEl.value : null,
            DateFrom: dateFromEl ? dateFromEl.value : null,
            DateTo: dateToEl ? dateToEl.value : null
        };
    }

    function populateDropdowns() {
        var $buildingSelect = $('#' + IDs.filterIds.building);
        var $typeSelect = $('#' + IDs.filterIds.type);

        if ($buildingSelect.length && $buildingSelect.find('option').length <= 1) {
            $buildingSelect.empty().append('<option value="0">Показать всё</option>');
            for (var key in mapBuildings) {
                if (mapBuildings.hasOwnProperty(key)) {
                    $buildingSelect.append(`<option value="${key}">${mapBuildings[key]}</option>`);
                }
            }
        }

        if ($typeSelect.length && $typeSelect.find('option').length <= 1) {
            $typeSelect.empty().append('<option value="0">Показать всё</option>');
            for (var key in mapTypes) {
                if (mapTypes.hasOwnProperty(key)) {
                    $typeSelect.append(`<option value="${key}">${mapTypes[key]}</option>`);
                }
            }
        }
    }

    function loadGrid() {
        var f = currentFilter();
        var query = qs({
            buildingFilter: f.BuildingFilter === '0' ? null : f.BuildingFilter,
            nodeNameFilter: f.NodeNameFilter,
            typeFilter: f.TypeFilter === '0' ? null : f.TypeFilter,
            deviceCount: f.DeviceCount,
            dateFromFilter: f.DateFrom,
            dateToFilter: f.DateTo,
            page: 1,
            pageSize: 1000
        });

        $.getJSON('/api/nodes?' + query)
            .done(renderGrid)
            .fail(function (xhr) { showError(xhr.statusText); });
    }

    function renderGrid(rows) {
        var $grid = $('#' + IDs.gridId);
        if (!$grid.length) { return; }

        if (!$grid.find('thead').length) {
            $grid.prepend(
                '<thead><tr>' +
                '<th>Корпус</th><th>Название</th><th>Тип</th>' +
                '<th>Доп.</th><th>Дата</th><th>Кол-во</th>' +
                '<th style="width:90px">Действия</th></tr></thead>');
        }

        var $tbody = $grid.find('tbody');
        if (!$tbody.length) {
            $tbody = $('<tbody></tbody>').appendTo($grid);
        }

        var body = [];
        for (var i = 0; i < rows.length; i++) {
            var r = rows[i];
            if (r.id === 0) continue; // исключить "не определён"

            body.push(
                '<tr class="data-row"' +
                ' data-buildname="' + (r.buildname || '') + '"' +
                ' data-name="' + (r.name || '') + '"' +
                ' data-typee="' + (r.typeName || '') + '"' +
                ' data-other="' + (r.other || '') + '"' +
                ' data-verification_date="' + (r.verificationDate || '') + '"' +
                ' data-cnt="' + (r.deviceCount || 0) + '"' +
                '>' +
                '<td>' + (r.buildname || '') + '</td>' +
                '<td>' + (r.name || '') + '</td>' +
                '<td>' + (r.typeName || '') + '</td>' +
                '<td>' + (r.other || '') + '</td>' +
                '<td>' + (r.verificationDate || '') + '</td>' +
                '<td>' + (r.deviceCount || 0) + '</td>' +
                '<td>' +
                    '<button class="btn btn-sm btn-primary btn-edit" data-id="' + r.id + '">Изменить</button> ' +
                    '<button class="btn btn-sm btn-danger btn-delete" data-id="' + r.id + '">Удалить</button>' +
                '</td>' +
                '</tr>');
        }
        $tbody.html(body.join(''));

        if ($('.js-group-checkbox:checked').length) { applyGroupingOnce(); }
    }

    function makeSelect(map, selected) {
        var html = '<select class="form-control form-control-sm">';
        for (var key in map) {
            if (!map.hasOwnProperty(key)) continue;
            var name = map[key];
            var sel = (String(key) === String(selected)) ? ' selected' : '';
            html += '<option value="' + key + '"' + sel + '>' + name + '</option>';
        }
        html += '</select>';
        return html;
    }

    // Изменить
    $(document).on('click', '.btn-edit', function (e) {
        e.preventDefault();

        var $tr = $(this).closest('tr');
        if ($tr.hasClass('editing')) return;
        $tr.addClass('editing');

        var d = $tr.data();             // jQuery читает data-* в объект

        // заменяем клетки на input/select
        $tr.children().eq(0).html(makeSelect(mapBuildings, d.build));
        $tr.children().eq(1).html('<input class="form-control form-control-sm" value="' + d.name + '" />');
        $tr.children().eq(2).html(makeSelect(mapTypes, d.type));
        $tr.children().eq(3).html('<textarea class="form-control form-control-sm">' + d.other + '</textarea>');
        $tr.children().eq(4).html('<input type="date" class="form-control form-control-sm" value="' + d.verification_date + '" />');
        $tr.children().eq(5).html('<input type="number" min="0" class="form-control form-control-sm" value="' + d.cnt + '" />');

        // действия
        $tr.children().eq(6).html(
            '<button type="button" class="btn btn-sm btn-success btn-save" data-id="' + d.id + '">Сохранить</button> ' +
            '<button type="button" class="btn btn-sm btn-secondary btn-cancel">Отмена</button>');
    });

    // Сохранить
    $(document).on('click', '.btn-save', function (e) {
        e.preventDefault();

        var $tr = $(this).closest('tr');
        var id = $(this).data('id');

        var dto = {
            BuildingId: +$tr.find('td:eq(0) select').val(),
            Name: $tr.find('td:eq(1) input').val().trim(),
            TypeId: +$tr.find('td:eq(2) select').val(),
            Other: $tr.find('td:eq(3) textarea').val().trim(),
            VerificationDate: $tr.find('td:eq(4) input').val() || null,
            DeviceCount: +$tr.find('td:eq(5) input').val()
        };

        $.ajax({
            url: '/api/nodes/' + id,
            type: 'PUT',
            data: JSON.stringify(dto),
            contentType: 'application/json'
        }).done(function () {
            loadGrid();                 // перерисовываем
        }).fail(function (xhr) {
            showError(xhr.responseText || 'Ошибка при сохранении');
        });
    });

    // Отмена
    $(document).on('click', '.btn-cancel', function (e) {
        e.preventDefault();
        loadGrid();
    });

    // Удалить
    $(document).on('click', '.btn-delete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        if (!confirm('Удалить узел?')) return;

        $.ajax({ url: '/api/nodes/' + id, type: 'DELETE' })
            .done(loadGrid)
            .fail(function () { showError('Ошибка при удалении'); });
    });


    // один раз создаём ссылку на applyGrouping из initializeGrouping
    var applyGroupingOnce = function () { };

    /* ------------------------------------------------------------
                    ГРУППИРОВКА
    ------------------------------------------------------------ */
    function initializeGrouping() {
        var mapping = {
            buildname: 'Корпус',
            name: 'Название узла',
            typee: 'Тип узла',
            verification_date: 'Дата проверки',
            cnt: 'Количество устройств'
        };

        // data‑column в чекбоксы
        var groupBuildingEl = $id(IDs.chkIds.col1);
        var groupNameEl = $id(IDs.chkIds.col2);
        var groupTypeEl = $id(IDs.chkIds.col3);
        var groupDateEl = $id(IDs.chkIds.col4);
        var groupDevicesEl = $id(IDs.chkIds.col5);

        if (groupBuildingEl) $(groupBuildingEl).attr('data-column', 'buildname');
        if (groupNameEl) $(groupNameEl).attr('data-column', 'name');
        if (groupTypeEl) $(groupTypeEl).attr('data-column', 'typee');
        if (groupDateEl) $(groupDateEl).attr('data-column', 'verification_date');
        if (groupDevicesEl) $(groupDevicesEl).attr('data-column', 'cnt');

        $('.group-checkbox').addClass('js-group-checkbox');
        $('.js-group-checkbox').on('change', applyGrouping);

        // доступно снаружи, чтобы вызвать после перерисовки
        applyGroupingOnce = applyGrouping;

        function applyGrouping() {
            var $grid = $('#' + IDs.gridId);
            var $tbody = $grid.find('tbody');
            if (!$tbody.length) { return; }

            // вернуть строки
            $tbody.find('.data-row').appendTo($tbody);
            $tbody.find('.group-header,.group-content').remove();

            // выбранные колонки
            var cols = [];
            $('.js-group-checkbox:checked').each(function () {
                cols.push($(this).data('column'));
            });
            if (!cols.length) { return; }

            // подготовить строки
            var groups = {};
            $tbody.find('tr.data-row').each(function () {
                var $r = $(this);
                var key = [];
                for (var i = 0; i < cols.length; i++) {
                    var val = $r.data(cols[i]) || 'Н/Д';
                    key.push(val);
                }
                key = key.join('|');
                if (!groups[key]) { groups[key] = []; }
                groups[key].push(this);
            });

            var colspan = $grid.find('th').length;

            for (var gKey in groups) {
                if (!groups.hasOwnProperty(gKey)) { continue; }

                var rows = groups[gKey];
                var firstRow = $(rows[0]);
                var header = [];
                for (var c = 0; c < cols.length; c++) {
                    if (c) { header.push(' | '); }
                    header.push(mapping[cols[c]] + ': ' + (firstRow.data(cols[c]) || 'Н/Д'));
                }
                header.push(' (' + rows.length + ')');

                var $head = $('<tr class="group-header"><td colspan="' + colspan + '"><i class="fa fa-plus-circle group-icon"></i> ' + header.join('') + '</td></tr>');
                var $content = $('<tbody class="group-content" style="display:none;"></tbody>');
                for (var r = 0; r < rows.length; r++) { $content.append(rows[r]); }

                $head.on('click', (function ($c, $h) {
                    return function () {
                        var open = $c.toggle().is(':visible');
                        var $i = $h.find('.group-icon');
                        $i.toggleClass('fa-plus-circle', !open).toggleClass('fa-minus-circle', open);
                    };
                })($content, $head));

                $tbody.append($head).append($content);
            }
        }
    }

    /* ------------------------------------------------------------
       ИНИЦИАЛИЗАЦИЯ
    ------------------------------------------------------------ */
    function bindFilterEvents() {
        if (!IDs.filterIds) return;

        var filterElements = [
            IDs.filterIds.building,
            IDs.filterIds.name,
            IDs.filterIds.type,
            IDs.filterIds.device,
            IDs.filterIds.dateFrom,
            IDs.filterIds.dateTo
        ];

        filterElements.forEach(function (id) {
            var el = $id(id);
            if (el) {
                el.addEventListener('change', loadGrid, false);
                if (el.type === 'text') {
                    el.addEventListener('keyup', function () {
                        clearTimeout(this.searchTimeout);
                        this.searchTimeout = setTimeout(loadGrid, 300);
                    }, false);
                }
            }
        });
    }

    function init(opts) {
        // ids из aspx
        IDs.gridId = opts.gridId;
        IDs.treeData = opts.treeData;
        IDs.chkIds = opts.chkIds || {};
        IDs.filterIds = opts.filterIds || {};

        // DOM
        els.srcSel = $id(opts.srcSel);
        els.fldBox = $id(opts.fldBox);
        els.genBtn = $id(opts.genBtn);
        els.expBtn = $id(opts.expBtn);
        els.resultEl = $id(opts.resultEl);
        els.loadEl = $id(opts.loadEl);

        // события отчётного конструктора (оставляем без изменений)
        if (els.srcSel) els.srcSel.addEventListener('change', loadFields);
        if (els.fldBox) els.fldBox.addEventListener('change', toggleButtons);
        if (els.genBtn) els.genBtn.addEventListener('click', getReport);
        if (els.expBtn) els.expBtn.addEventListener('click', exportReport);


        // Подгрузка списка источников для отчётного конструктора
        if (els.srcSel) {
            serverCall('GetConfig')
                .then(function (cfg) {
                    var html = '<option value="">-- Выберите источник --</option>';
                    for (var i = 0; i < cfg.AvailableSources.length; i++) {
                        html += '<option value="' + cfg.AvailableSources[i] + '">' + cfg.AvailableSources[i] + '</option>';
                    }
                    els.srcSel.innerHTML = html;
                })
                .catch(function () { /* ignore */ });
        }

        // когда DOM и jQuery готовы
        $(function () {
            bindFilterEvents();
            initializeGrouping();

            loadInitialData().then(function () {
                populateDropdowns();
                loadGrid();
                loadTree();
            });
        });
    }

    // публичный интерфейс
    return {
        init: init
    };
})();
