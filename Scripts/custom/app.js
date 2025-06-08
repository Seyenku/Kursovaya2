const CN_App = ((api, filters, tree, grid, modal) => {
    let config;

    const enrichNodeData = nodes => {
        nodes.forEach(node => {
            node.buildingName = filters.getBuildingName(node.building);
            node.typeName = filters.getTypeName(node.type);
        });
        return nodes;
    };

    const loadAndDisplayNodes = () => {
        const filterValues = filters.getValues();
        api.nodes(filterValues).done(nodes => {
            const enrichedNodes = enrichNodeData(nodes);
            grid.render(enrichedNodes);
        });
    };

    return {
        start: appConfig => {
            config = appConfig;

            filters.init(config.filterIds, loadAndDisplayNodes)
                .then(() => {
                    grid.init(config.gridId);
                    modal.init();
                    tree.init(nodeId => {
                        // Логика выделения узла в таблице:)
                    });
                    loadAndDisplayNodes();
                });
        },
        refresh: loadAndDisplayNodes
    };
})(CN_Api, CN_Filters, CN_Tree, CN_Grid, CN_Modal);