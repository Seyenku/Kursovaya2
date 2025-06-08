const CN_Filters = ((util, api) => {
    let config = {};
    const maps = { buildings: {}, types: {} };

    const fillSelect = (selector, dataMap) => {
        const $select = $(selector);
        $select.empty().append('<option value="0">Показать всё</option>');
        Object.entries(dataMap).forEach(([id, name]) =>
            $select.append(`<option value="${id}">${name}</option>`)
        );
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

            $element.on('change', callback);

            if ($element.attr('type') === 'text' || $element.is('input[type="text"]')) {
                $element.on('keyup', function () {
                    clearTimeout(this.timer);
                    this.timer = setTimeout(callback, 300);
                });
            }
        });

        $('#FilterPeriod').on('select2:select select2:unselect select2:clear change', callback);
    };

    const loadData = () => $.when(
        api.buildings().done(data => {
            data.forEach(item => maps.buildings[item.id] = item.name);
            fillSelect('#' + config.building, maps.buildings);
        }),
        api.nodeTypes().done(data => {
            data.forEach(item => maps.types[item.id] = item.name);
            fillSelect('#' + config.type, maps.types);
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