const CN_Api = (util => {
    const get = (url, params) => $.getJSON(url + (params ? '?' + util.encode(params) : ''));
    const post = (url, data) => util.ajax({
        url,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json'
    });
    const put = (url, data) => util.ajax({
        url,
        type: 'PUT',
        data: JSON.stringify(data),
        contentType: 'application/json'
    });
    const del = url => util.ajax({ url, type: 'DELETE' });

    return {
        nodes: params => get('/api/nodes', params),
        nodeTree: () => get('/api/nodes/tree'),
        buildings: () => get('/api/buildings'),
        nodeTypes: () => get('/api/nodetypes'),
        createNode: data => post('/api/nodes', data),
        updateNode: (id, data) => put(`/api/nodes/${id}`, data),
        deleteNode: id => del(`/api/nodes/${id}`)
    };
})(CN_Util);