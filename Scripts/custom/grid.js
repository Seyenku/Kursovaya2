const CN_Grid = ((util, api, filters) => {
    let $tbody;

    const createSelect = (options, selectedValue) => {
        const optionsHtml = Object.entries(options)
            .map(([value, text]) =>
                `<option value="${value}"${value == selectedValue ? ' selected' : ''}>${text}</option>`
            )
            .join('');
        return `<select class="form-control form-control-sm">${optionsHtml}</select>`;
    };

    const createRow = node => `
        <tr class="data-row" data-id="${node.id}" data-building="${node.building}"
            data-name="${node.name}"
            data-type="${node.type}" data-other="${node.other || ''}"
            data-date="${node.verificationDate || ''}" data-count="${node.deviceCount || 0}">
            <td>${node.buildingName}</td>
            <td>${node.name}</td>
            <td>${node.typeName}</td>
            <td>${node.other || ''}</td>
            <td>${node.verificationDate || ''}</td>
            <td>${node.deviceCount}</td>
            <td>
                <button class="btn btn-sm btn-primary btn-edit"><i class="fa fa-edit"></i></button>
                <button class="btn btn-sm btn-danger btn-delete"><i class="fa fa-trash"></i></button>
            </td>
        </tr>`;

    const renderRows = nodes => {
        $tbody.html(nodes.filter(node => node.id).map(createRow).join(''));
    };

    const editRow = row => {
        const $row = $(row);
        const data = $row.data();

        $row.addClass('editing');
        $row.children()
            .eq(0).html(createSelect(filters.getBuildingMap(), data.building))
            .end().eq(1).html(`<input class="form-control form-control-sm" value="${data.name || ''}">`)
            .end().eq(2).html(createSelect(filters.getTypeMap(), data.type))
            .end().eq(3).html(`<textarea class="form-control form-control-sm">${data.other || ''}</textarea>`)
            .end().eq(4).html(`<input type="date" class="form-control form-control-sm" value="${data.date || ''}">`)
            .end().eq(5).html(`<input type="number" min="0" class="form-control form-control-sm" value="${data.count || 0}">`)
            .end().eq(6).html(`
                <button class="btn btn-sm btn-success btn-save"><i class="fa-solid fa-floppy-disk"></i></button>
                <button class="btn btn-sm btn-secondary btn-cancel"><i class="fa-solid fa-rectangle-xmark"></i></button>
            `);
    };

    const collectRowData = row => {
        const $row = $(row);
        return {
            BuildingId: parseInt($row.find('td:eq(0) select').val()),
            Name: $row.find('td:eq(1) input').val().trim(),
            TypeId: parseInt($row.find('td:eq(2) select').val()),
            Other: $row.find('td:eq(3) textarea').val().trim(),
            VerificationDate: $row.find('td:eq(4) input').val() || null,
            DeviceCount: parseInt($row.find('td:eq(5) input').val())
        };
    };

    const bindEvents = gridId => {
        $(document)
            .on('click', `#${gridId} .btn-edit`, function () {
                editRow(this.closest('tr'));
            })
            .on('click', `#${gridId} .btn-cancel`, () => {
                CN_App.refresh();
            })
            .on('click', `#${gridId} .btn-save`, function () {
                const row = this.closest('tr');
                const id = $(row).data('id');
                const data = collectRowData(row);
                api.updateNode(id, data).done(() => CN_App.refresh());
            })
            .on('click', `#${gridId} .btn-delete`, function () {
                const id = $(this).closest('tr').data('id');
                if (confirm('Удалить узел?')) {
                    api.deleteNode(id).done(() => CN_App.refresh());
                }
            });
    };

    return {
        init: gridId => {
            $tbody = $(`#${gridId} tbody`);
            bindEvents(gridId);
        },
        render: renderRows
    };
})(CN_Util, CN_Api, CN_Filters);