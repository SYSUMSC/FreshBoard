export function Post(url, headers = {}, data = {}) {
    var myFormData = new FormData();
    for (var x in data) {
        myFormData.append(x, data[x]);
    }

    var myHeaders = new Headers();
    for (var x in headers) {
        myHeaders.append(x, headers[x]);
    }

    var myInit = {
        method: 'POST',
        headers: myHeaders,
        credentials: "same-origin",
        data: myFormData,
        mode: 'cors',
        cache: 'default'
    };

    var myRequest = new Request(url);

    return fetch(myRequest, myInit);
}

export function Get(url, headers = {}, data = {}) {
    var paramStr = '?';
    for (var x in data) {
        paramStr += `${x}=${escape(data[x])}&`;
    }

    var myHeaders = new Headers();
    for (var x in headers) {
        myHeaders.append(x, headers[x]);
    }

    var myInit = {
        method: 'GET',
        headers: myHeaders,
        credentials: "same-origin",
        mode: 'cors',
        cache: 'default'
    };

    var myRequest = new Request(url + paramStr);

    return fetch(myRequest, myInit);
}
