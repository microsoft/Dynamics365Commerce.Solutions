System.register(["PosApi/Create/RequestHandlers", "PosApi/TypeExtensions", "../Messages/ExecuteHttpRequestRequest", "../Messages/ExecuteHttpRequestResponse", "../Infrastructure/Http/HttpStatusCodes", "../Infrastructure/Http/HttpRequestError", "../Infrastructure/Http/HttpMethods"], function (exports_1, context_1) {
    "use strict";
    var __extends = (this && this.__extends) || (function () {
        var extendStatics = function (d, b) {
            extendStatics = Object.setPrototypeOf ||
                ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
                function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
            return extendStatics(d, b);
        };
        return function (d, b) {
            extendStatics(d, b);
            function __() { this.constructor = d; }
            d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
        };
    })();
    var RequestHandlers_1, TypeExtensions_1, ExecuteHttpRequestRequest_1, ExecuteHttpRequestResponse_1, HttpStatusCodes_1, HttpRequestError_1, HttpMethods_1, ExecuteHttpRequestRequestHandler;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (RequestHandlers_1_1) {
                RequestHandlers_1 = RequestHandlers_1_1;
            },
            function (TypeExtensions_1_1) {
                TypeExtensions_1 = TypeExtensions_1_1;
            },
            function (ExecuteHttpRequestRequest_1_1) {
                ExecuteHttpRequestRequest_1 = ExecuteHttpRequestRequest_1_1;
            },
            function (ExecuteHttpRequestResponse_1_1) {
                ExecuteHttpRequestResponse_1 = ExecuteHttpRequestResponse_1_1;
            },
            function (HttpStatusCodes_1_1) {
                HttpStatusCodes_1 = HttpStatusCodes_1_1;
            },
            function (HttpRequestError_1_1) {
                HttpRequestError_1 = HttpRequestError_1_1;
            },
            function (HttpMethods_1_1) {
                HttpMethods_1 = HttpMethods_1_1;
            }
        ],
        execute: function () {
            ExecuteHttpRequestRequestHandler = (function (_super) {
                __extends(ExecuteHttpRequestRequestHandler, _super);
                function ExecuteHttpRequestRequestHandler() {
                    return _super !== null && _super.apply(this, arguments) || this;
                }
                ExecuteHttpRequestRequestHandler.prototype.supportedRequestType = function () {
                    return ExecuteHttpRequestRequest_1.ExecuteHttpRequestRequest;
                };
                ExecuteHttpRequestRequestHandler.prototype.executeAsync = function (request) {
                    return ExecuteHttpRequestRequestHandler._executeHttpRequest(request.httpRequest).then(function (httpResponse) {
                        return {
                            canceled: false,
                            data: new ExecuteHttpRequestResponse_1.ExecuteHttpRequestResponse(httpResponse)
                        };
                    });
                };
                ExecuteHttpRequestRequestHandler._executeHttpRequest = function (request) {
                    var promise = new Promise(function (resolve, reject) {
                        var xhr = new XMLHttpRequest();
                        xhr.ontimeout = function () {
                            if (xhr != null) {
                                xhr.abort();
                                xhr = null;
                                var statusText = "Request Timeout";
                                var response = {
                                    ok: false,
                                    statusCode: HttpStatusCodes_1.HttpStatusCodes.TIMEOUT,
                                    statusText: statusText,
                                    headers: {},
                                    body: TypeExtensions_1.StringExtensions.EMPTY
                                };
                                reject(new HttpRequestError_1.HttpRequestError(statusText, response));
                            }
                        };
                        xhr.onloadend = function () {
                            if (xhr === null) {
                                return;
                            }
                            var response = ExecuteHttpRequestRequestHandler._getResponse(xhr);
                            xhr = null;
                            if (response.ok) {
                                resolve(response);
                            }
                            else {
                                reject(new HttpRequestError_1.HttpRequestError(response.statusText, response));
                            }
                        };
                        xhr.onerror = function () {
                            var message = ExecuteHttpRequestRequestHandler._getErrorMessage();
                            reject(new HttpRequestError_1.HttpRequestError(message));
                        };
                        try {
                            xhr.open(request.method || HttpMethods_1.HttpMethods.GET, request.requestUri, true);
                        }
                        catch (exception) {
                            var message = ExecuteHttpRequestRequestHandler._getErrorMessage(exception);
                            reject(new HttpRequestError_1.HttpRequestError(message));
                            return;
                        }
                        if (request.headers) {
                            TypeExtensions_1.ObjectExtensions.forEachKeyValuePair(request.headers, function (headerName, headerValue) {
                                xhr.setRequestHeader(headerName, headerValue);
                            });
                        }
                        xhr.timeout = request.timeout;
                        try {
                            xhr.send(request.body);
                        }
                        catch (exception) {
                            var message = ExecuteHttpRequestRequestHandler._getErrorMessage(exception);
                            reject(new HttpRequestError_1.HttpRequestError(message));
                        }
                    });
                    return promise;
                };
                ExecuteHttpRequestRequestHandler._parseXmlHttpResponseHeaders = function (headerString) {
                    var responseHeaders = (headerString || "").split(/\r?\n/);
                    var headers = {};
                    responseHeaders.forEach(function (value) {
                        if (value) {
                            var pair = value.split(": ");
                            headers[pair[0]] = pair[1];
                        }
                    });
                    return headers;
                };
                ExecuteHttpRequestRequestHandler._getErrorMessage = function (error) {
                    var errorMessage = "Failed to send the HTTP request.";
                    if (!TypeExtensions_1.ObjectExtensions.isNullOrUndefined(error) && !TypeExtensions_1.ObjectExtensions.isNullOrUndefined(error.message)) {
                        errorMessage = error.message;
                    }
                    return errorMessage;
                };
                ExecuteHttpRequestRequestHandler._getResponse = function (xhr) {
                    var statusText = xhr.statusText;
                    var statusCode = xhr.status;
                    if (statusCode === 1223) {
                        statusCode = HttpStatusCodes_1.HttpStatusCodes.NO_CONTENT;
                        statusText = "No Content";
                    }
                    return {
                        statusCode: statusCode,
                        statusText: statusText,
                        headers: this._parseXmlHttpResponseHeaders(xhr.getAllResponseHeaders()),
                        body: xhr.responseText,
                        ok: HttpStatusCodes_1.HttpStatusCodes.isSuccessful(statusCode)
                    };
                };
                return ExecuteHttpRequestRequestHandler;
            }(RequestHandlers_1.ExtensionRequestHandlerBase));
            exports_1("default", ExecuteHttpRequestRequestHandler);
        }
    };
});
//# sourceMappingURL=C:/__w/1/s/r/src/FiscalIntegration/PosFiscalConnectorSample/Pos.Extension/RequestHandlers/ExecuteHttpRequestRequestHandler.js.map