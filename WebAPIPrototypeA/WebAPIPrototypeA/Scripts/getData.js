var targetUrl;

function getData(url) {
    targetUrl = url;
};

getData.prototype.getForJsonResponse = function (callback) {
    var request = new XMLHttpRequest();
    request.open("GET", targetUrl, true);
    request.onreadystatechange = function () {
        if (request.readyState != 4 || request.status != 200)
            return;
        callback(request.responseText);
    };
    request.send();
}

getData.prototype.postForJsonResponse = function (data, callback) {
    var request = new XMLHttpRequest();
    request.open("POST", targetUrl, true);
    request.setRequestHeader("Accept", "application/json");
    request.setRequestHeader("Content-type", "application/json");
    request.setRequestHeader("Content-length", data.length);
    request.setRequestHeader("Connection", "close");
    request.onreadystatechange = function () {
        if (request.readyState != 4 || request.status != 200)
            return;
        callback(request.responseText);
    };
    request.send(data);
}