const CN_Tree = ((util, api) => {
    const $tree = $('#nodesTree');

    const renderTree = data => {
        $tree.jstree('destroy').jstree({
            core: { data },
            plugins: ['state', 'wholerow', 'search']
        });
    };

    const bindTreeEvents = onSelect => {
        $tree.on('select_node.jstree', (event, data) => {
            if (data.node.id.startsWith('node_')) {
                onSelect(data.node.data.nodeId);
            }
        });
    };

    const bindUIEvents = () => {
        $tree.on('ready.jstree', () => {
            $('#treeSearchInput').on('keyup', function () {
                $tree.jstree(true).search(this.value);
            });
        });

        $('#treeToggleBtn').on('click', () => {
            $('#treeCollapse').collapse('toggle');
        });

        $('#treeCollapse')
            .on('show.bs.collapse', () => {
                $('.collapse-icon').removeClass('fa-chevron-right').addClass('fa-chevron-down');
            })
            .on('hide.bs.collapse', () => {
                $('.collapse-icon').removeClass('fa-chevron-down').addClass('fa-chevron-right');
            });
    };

    return {
        init: onSelect => {
            api.nodeTree().done(renderTree);
            bindTreeEvents(onSelect);
            bindUIEvents();
        }
    };
})(CN_Util, CN_Api);