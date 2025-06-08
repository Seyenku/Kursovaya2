const CN_Util = (() => {
    const get = id => document.getElementById(id);
    const $ = selector => jQuery(selector);

    const encode = params => Object.entries(params)
        .filter(([_, value]) => value != null && value !== '')
        .flatMap(([key, value]) =>
            Array.isArray(value)
                ? value.map(v => `${encodeURIComponent(key)}=${encodeURIComponent(v)}`)
                : [`${encodeURIComponent(key)}=${encodeURIComponent(value)}`]
        )
        .join('&');

    const ajax = options => $.ajax(options).fail(error =>
        alert(error.responseText || 'Ошибка'));

    return { $, get, encode, ajax };
})();
