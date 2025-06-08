const CN_Modal = ((util, api, filters) => {
    let $modal, formElements;

    const initFormElements = () => {
        formElements = {
            name: util.$('FormInputName'),
            date: util.$('FormInputDate'),
            building: util.$('FormDropBuilding'),
            type: util.$('FormDropType'),
            other: util.$('FormInputOther')
        };
    };

    const fillDropdowns = () => {
        const $buildingSelect = $('#FormDropBuilding');
        $buildingSelect.empty();
        Object.entries(filters.getBuildingMap()).forEach(([id, name]) => {
            $buildingSelect.append(`<option value="${id}">${name}</option>`);
        });

        const $typeSelect = $('#FormDropType');
        $typeSelect.empty();
        Object.entries(filters.getTypeMap()).forEach(([id, name]) => {
            $typeSelect.append(`<option value="${id}">${name}</option>`);
        });
    };

    const clearForm = () => {
        Object.values(formElements).forEach(element => {
            if (element) element.value = '';
        });
    };

    const collectFormData = () => ({
        Name: formElements.name.value.trim(),
        BuildingId: parseInt(formElements.building.value),
        TypeId: parseInt(formElements.type.value),
        VerificationDate: formElements.date.value || null,
        Other: formElements.other.value.trim(),
        DeviceCount: 0
    });

    const validateForm = data => {
        if (!data.Name) {
            alert('Введите название узла');
            return false;
        }
        if (!data.BuildingId || data.BuildingId === 0) {
            alert('Выберите корпус');
            return false;
        }
        if (!data.TypeId || data.TypeId === 0) {
            alert('Выберите тип узла');
            return false;
        }
        return true;
    };

    const bindEvents = () => {
        $('#BtnAddNode').on('click', () => {
            fillDropdowns();
            clearForm();
            $modal.modal('show');
        });

        $('#BtnSaveNode').on('click', () => {
            const data = collectFormData();
            if (!validateForm(data)) return;

            api.createNode(data)
                .done(() => {
                    $modal.modal('hide');
                    CN_App.refresh();
                })
                .fail(error => {
                    alert('Ошибка при сохранении: ' + (error.responseText || 'Неизвестная ошибка'));
                });
        });

        $modal.on('hidden.bs.modal', clearForm);

        // Дополнительные обработчики для кнопок закрытия
        $modal.find('[data-dismiss="modal"]').on('click', () => {
            $modal.modal('hide');
        });

        $modal.on('click', function (e) {
            if (e.target === this) {
                $modal.modal('hide');
            }
        });
    };

    return {
        init: () => {
            $modal = $('#nodesNew');
            initFormElements();
            bindEvents();
        }
    };
})(CN_Util, CN_Api, CN_Filters);