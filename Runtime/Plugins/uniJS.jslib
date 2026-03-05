mergeInto(LibraryManager.library , {
    Lib_InstanceReady: function () {
        window.Unity._instanceReady();
    },

    Lib_StartListeningToClientEvents: function(callback) {
        window.Unity._registerClientListener(function(event, payload) {
            var eventBufferSize = lengthBytesUTF8(event) + 1;
            var eventBuffer = _malloc(eventBufferSize);
            stringToUTF8(event, eventBuffer, eventBufferSize);
            var payloadBufferSize = lengthBytesUTF8(payload) + 1;
            var payloadBuffer = _malloc(payloadBufferSize);
            stringToUTF8(payload, payloadBuffer, payloadBufferSize);
            var responseJson = {{{ makeDynCall('iii', 'callback') }}} (eventBuffer, payloadBuffer);
            _free(eventBuffer);
            _free(payloadBuffer);
            return UTF8ToString(responseJson);
        });
    },

    Lib_SendEventToClient: function (eventName, payloadJson) {
        window.Unity._receiveEvent(UTF8ToString(eventName), UTF8ToString(payloadJson));
    },

    Lib_RegisterKeyGameObject: function (key, dataJson) {
        var data = JSON.parse(UTF8ToString(dataJson));
        window.Unity.GameObject._register(UTF8ToString(key), data);
    },

    Lib_SendGameObjectLifeCycleEvent: function (key, event) {
        window.Unity.GameObject._receiveLifeCycleEvent(UTF8ToString(key), UTF8ToString(event));
    },

    Lib_LogToJS: function (verbosity, message) {
        window.Unity._logFromUnity(UTF8ToString(verbosity), UTF8ToString(message));
    }
});