const CN_Filters = ((util, api) => {
    let config = {};
    const maps = { buildings: {}, types: {} };

    const initSelect2 = (selector, dataMap, placeholder) => {
        const $select = $(selector);
        $select.empty();

        $select.select2({
            data: [
                { id: '0', text: 'Показать всё' },
                ...Object.entries(dataMap).map(([id, name]) => ({ id, text: name }))
            ],
            placeholder: placeholder || 'Выберите значение',
            allowClear: true,
            width: '100%',
            templateResult: (data) => {
                if (data.id === '0') {
                    return $('<span>').addClass('text-muted').text(data.text);
                }
                return data.text;
            }
        });

        // Устанавливаем значение по умолчанию
        $select.val('0').trigger('change');
    };

    const getFilterValues = () => ({
        BuildingFilter: getValue('building'),
        NodeNameFilter: getValue('name'),
        TypeFilter: getValue('type'),
        DeviceCount: getValue('device'),
        Periods: $('#FilterPeriod').val() || []
    });

    const getValue = key => {
        const element = document.getElementById(config[key]);
        if (!element) return null;
        const value = (element.value || '').toString().trim();
        return value === '0' || !value ? null : value;
    };

    const bindEvents = callback => {
        Object.entries(config).forEach(([filterName, elementId]) => {
            const $element = $('#' + elementId);
            if ($element.length === 0) return;

            // Для Select2 элементов используем специальные события
            if ($element.hasClass('select2-hidden-accessible') || filterName === 'building' || filterName === 'type') {
                $element.on('select2:select select2:unselect select2:clear change', callback);
            } else if ($element.attr('type') === 'text' || $element.is('input[type="text"]')) {
                $element.on('keyup', function () {
                    clearTimeout(this.timer);
                    this.timer = setTimeout(callback, 300);
                });
            } else {
                $element.on('change', callback);
            }
        });

        $('#FilterPeriod').on('select2:select select2:unselect select2:clear change', callback);
    };

    const loadData = () => $.when(
        api.buildings().done(data => {
            data.forEach(item => maps.buildings[item.id] = item.name);
            initSelect2('#' + config.building, maps.buildings, 'Выберите корпус');
        }),
        api.nodeTypes().done(data => {
            data.forEach(item => maps.types[item.id] = item.name);
            initSelect2('#' + config.type, maps.types, 'Выберите тип узла');
        })
    );

    const buildMonths = () => {
        const res = [], now = new Date();
        for (let y = 2010; y <= now.getFullYear(); y++) {
            for (let m = 1; m <= 12; m++) {
                const mm = m.toString().padStart(2, '0');
                res.push(`${mm}.${y}`);
            }
        }
        return res;
    };

    const loadFilterPeriod = () => {
        $('#FilterPeriod').select2({
            data: buildMonths().map(m => ({ id: m, text: m })),
            tags: true,
            tokenSeparators: [',', ' '],
            placeholder: 'Выберите месяц(ы)',
            width: '100%',
            createTag: params => {
                const re = /^(0[1-9]|1[0-2])\.\d{4}$/;
                return re.test(params.term)
                    ? { id: params.term, text: params.term, newOption: true }
                    : null;
            },
            templateResult: data => data.newOption
                ? $('<span>').text(`${data.text} *`)
                : data.text
        });
    };

    return {
        init: (filterConfig, callback) => {
            config = filterConfig;
            return loadData().then(() => {
                loadFilterPeriod();
                bindEvents(callback);
            });
        },
        getValues: getFilterValues,
        getBuildingName: id => maps.buildings[id],
        getTypeName: id => maps.types[id],
        getBuildingMap: () => maps.buildings,
        getTypeMap: () => maps.types
    };
})(CN_Util, CN_Api);