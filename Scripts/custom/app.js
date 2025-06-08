const CN_App = ((api, filters, tree, grid, modal) => {
    let config;
    const paging = { page: 1, size: 20 };

    const enrichNodeData = nodes => {
        nodes.forEach(node => {
            node.buildingName = filters.getBuildingName(node.building);
            node.typeName = filters.getTypeName(node.type);
        });
        return nodes;
    };

    const loadAndDisplayNodes = () => {
        const filterValues = filters.getValues();
        api.nodes({ ...filterValues, ...paging })
            .done(({ items, total }) => {
                const enriched = enrichNodeData(items);
                grid.render(enriched);
                if (window.CN_Pager) {
                    CN_Pager.render(paging.page, paging.size, total);
                }
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
                        // Логика выделения узла в таблице
                    });

                    if (window.CN_Pager) {
                        CN_Pager.init('pager', () => {
                            loadAndDisplayNodes();
                        });
                        CN_Pager.setPaging(paging);
                    }

                    loadAndDisplayNodes();
                });
        },
        refresh: loadAndDisplayNodes,
        getPaging: () => paging
    };
})(CN_Api, CN_Filters, CN_Tree, CN_Grid, CN_Modal);