export function FormPost(url, form) {
    var myInit = {
        method: 'POST',
        credentials: "same-origin",
        body: new FormData(form),
        mode: 'cors',
        cache: 'default'
    };

    var myRequest = new Request(url);

    return fetch(myRequest, myInit);
}

export function Post(url, headers = {}, data = {}) {
    var myFormData = new FormData();
    for (var x in data) {
        myFormData.append(x, data[x]);
    }

    var myHeaders = new Headers();
    for (var y in headers) {
        myHeaders.append(y, headers[y]);
    }

    var myInit = {
        method: 'POST',
        headers: myHeaders,
        credentials: "same-origin",
        body: myFormData,
        mode: 'cors',
        cache: 'default'
    };

    var myRequest = new Request(url);

    return fetch(myRequest, myInit);
}
export function Get(url, headers = {}, data = {}) {
    var paramStr = '?';
    for (var x in data) {
        paramStr += `${x}=${encodeURIComponent(data[x])}&`;
    }

    var myHeaders = new Headers();
    for (var y in headers) {
        myHeaders.append(y, headers[y]);
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
