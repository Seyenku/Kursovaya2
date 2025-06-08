const CN_Pager = (() => {
    let $container;
    let paging;
    let callback;

    const render = (page, size, total) => {
        const pages = Math.ceil(total / size);
        if (pages <= 1) {
            $container.html('');
            return;
        }

        const buttons = Array.from({ length: pages }, (_, i) =>
            `<button class="btn btn-sm ${i + 1 === page ? 'btn-primary' : 'btn-light'}"
                     data-page="${i + 1}">${i + 1}</button>`
        ).join('');
        $container.html(buttons);
    };

    const bind = () => {
        $(document).on('click', '#pager button', function () {
            paging.page = parseInt($(this).data('page'));
            callback();
        });
    };

    return {
        init: (containerId, callbackFn) => {
            $container = $('#' + containerId);
            callback = callbackFn;
            bind();
        },
        render,
        setPaging: pagingRef => {
            paging = pagingRef;
        }
    };
})();

window.CN_Pager = CN_Pager;