const CN_Util = (() => {
    const $ = id => document.getElementById(id);
    const encode = params => Object.entries(params)
        .filter(([_, value]) => value != null && value !== '')
        .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
        .join('&');
    const ajax = options => $.ajax(options).fail(error => alert(error.responseText || 'Ошибка'));

    return { $, encode, ajax };
})();