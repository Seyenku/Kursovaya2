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
        DateFrom: getValue('dateFrom'),
        DateTo: getValue('dateTo')
    });

    const getValue = key => {
        const element = util.$(config[key]);
        if (!element) return null;
        const value = element.value.trim();
        return value === '0' || !value ? null : value;
    };

    const bindEvents = callback => {
        Object.values(config).forEach(id => {
            const element = util.$(id);
            if (!element) return;

            element.addEventListener('change', callback);
            if (element.type === 'text') {
                element.addEventListener('keyup', function () {
                    clearTimeout(this.timer);
                    this.timer = setTimeout(callback, 300);
                });
            }
        });
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

    return {
        init: (filterConfig, callback) => {
            config = filterConfig;
            bindEvents(callback);
            return loadData();
        },
        getValues: getFilterValues,
        getBuildingName: id => maps.buildings[id],
        getTypeName: id => maps.types[id],
        getBuildingMap: () => maps.buildings,
        getTypeMap: () => maps.types
    };
})(CN_Util, CN_Api);