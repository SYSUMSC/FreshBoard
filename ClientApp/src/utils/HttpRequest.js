export class HttpRequest {
    Post(url, headers, data) {
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
            data: myFormData,
            mode: 'cors',
            cache: 'default'
        };

        var myRequest = new Request(url);

        return fetch(myRequest, myInit);
    }

    Get(url, headers, data) {
        var myFormData = new FormData();
        for (var x in data) {
            myFormData.append(x, data[x]);
        }

        var myHeaders = new Headers();
        for (var x in headers) {
            myHeaders.append(x, headers[x]);
        }

        var myInit = {
            method: 'GET',
            headers: myHeaders,
            data: myFormData,
            mode: 'cors',
            cache: 'default'
        };

        var myRequest = new Request(url);

        return fetch(myRequest, myInit);
    }
}