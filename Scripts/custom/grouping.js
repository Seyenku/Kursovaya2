const CN_Grouping = (() => {
    let activeGroups = [];
    let onChangeCallback = null;
    let collapsedGroups = new Set();

    const groupTypes = {
        building: {
            name: 'Корпус',
            getValue: node => node.buildingName
        },
        name: {
            name: 'Первая буква названия',
            getValue: node => node.name?.charAt(0)?.toUpperCase() || 'Без названия'
        },
        type: {
            name: 'Тип узла',
            getValue: node => node.typeName
        },
        date: {
            name: 'Год проверки',
            getValue: node => node.verificationDate ?
                new Date(node.verificationDate).getFullYear().toString() : 'Без даты'
        },
        devices: {
            name: 'Диапазон устройств',
            getValue: node => node.deviceCount > 0 ?
                `${Math.floor(node.deviceCount / 10) * 10}-${Math.floor(node.deviceCount / 10) * 10 + 9}` : '0'
        }
    };

    const getActiveGroups = () => {
        activeGroups = [];
        $('.group-checkbox:checked').each(function () {
            const groupType = this.id.replace('GroupBy', '').toLowerCase();
            if (groupTypes[groupType]) {
                activeGroups.push(groupType);
            }
        });
        return activeGroups;
    };

    const createGroupKey = node => {
        return activeGroups.map(group =>
            `${groupTypes[group].name}: ${groupTypes[group].getValue(node)}`
        ).join(' / ');
    };

    const groupData = nodes => {
        if (activeGroups.length === 0) {
            return { ungrouped: nodes };
        }

        return nodes.reduce((acc, node) => {
            const key = createGroupKey(node);
            if (!acc[key]) {
                acc[key] = [];
            }
            acc[key].push(node);
            return acc;
        }, {});
    };

    const toggleGroup = groupKey => {
        if (collapsedGroups.has(groupKey)) {
            collapsedGroups.delete(groupKey);
        } else {
            collapsedGroups.add(groupKey);
        }
        if (onChangeCallback) {
            onChangeCallback();
        }
    };

    const isGroupCollapsed = groupKey => {
        return collapsedGroups.has(groupKey);
    };

    const bindEvents = callback => {
        onChangeCallback = callback;
        $('.group-checkbox').on('change', () => {
            getActiveGroups();
            if (onChangeCallback) {
                onChangeCallback();
            }
        });
    };

    return {
        init: bindEvents,
        hasActiveGroups: () => activeGroups.length > 0,
        groupData,
        getActiveGroups,
        toggleGroup,
        isGroupCollapsed
    };
})();