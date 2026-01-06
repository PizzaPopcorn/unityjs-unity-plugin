# UnityJS
This library will let you interact with your Unity WebGL build from JS in a more efficient way than using SendMessage. It also includes a JavaScript library for using all available unity functions.

If you are developing a web game that can't run in editor because it depends on browser stuff or you simply want to prototype fast directly in the browser, this plugin is for you.

## How to use
1. Install this repo as a package in your Unity project using the package manager.
2. Add a null object to your starting scene and drag the `JSInstance.cs` behavior to it.
3. Drag the `JSKeyGameObject` to each object you want to expose directly to JavaScript and set a key to identify it. You will be able to access any object down in its hierarchy from JS.
4. Recommended: To maximize the fast prototyping potencial, export all of your game content to asset bundles, that way you don't have to build the game each time, just update your bundles.
5. Build your game in WebGL
6. Copy the code below and paste it in a new JavaScript file in your web page. You can name it `unityInterop.js`
```js
class GameObject {

    static keyGameObjects = {};
    static #lifeCycleCallbacks = {
        awake: {},
        start: {},
        enable: {},
        disable: {},
        destroy: {}
    }

    static GetKeyGameObject(key){
        if(!GameObject.keyGameObjects.hasOwnProperty(key)) {
            console.error(`GameObject with key '${key}' not found`);
            return null;
        }
        return GameObject.keyGameObjects[key];
    }
    
    static onAwake(key, callback){
        if(!GameObject.#lifeCycleCallbacks.awake.hasOwnProperty(key)) {
            GameObject.#lifeCycleCallbacks.awake[key] = new Set();
        }
        GameObject.#lifeCycleCallbacks.awake[key].add(callback);
    }
    
    static onStart(key, callback) {
        if(!GameObject.#lifeCycleCallbacks.start.hasOwnProperty(key)) {
            GameObject.#lifeCycleCallbacks.start[key] = new Set();
        }
        GameObject.#lifeCycleCallbacks.start[key].add(callback);
    }
    
    static onEnable(key, callback) {
        if(!GameObject.#lifeCycleCallbacks.enable.hasOwnProperty(key)) {
            GameObject.#lifeCycleCallbacks.enable[key] = new Set();
        }
        GameObject.#lifeCycleCallbacks.enable[key].add(callback);
    }
    
    static onDisable(key, callback) {
        if(!GameObject.#lifeCycleCallbacks.disable.hasOwnProperty(key)) {
            GameObject.#lifeCycleCallbacks.disable[key] = new Set();
        }
        GameObject.#lifeCycleCallbacks.disable[key].add(callback);
    }
    
    static onDestroy(key, callback) {
        if(!GameObject.#lifeCycleCallbacks.destroy.hasOwnProperty(key)) {
            GameObject.#lifeCycleCallbacks.destroy[key] = new Set();
        }
        GameObject.#lifeCycleCallbacks.destroy[key].add(callback);
    }
    
    static _register(key, data) {
        GameObject.keyGameObjects[key] = new GameObject(key, data);
    }
    
    static _receiveLifeCycleEvent(key, event) {
        const gameObject = GameObject.keyGameObjects[key];
        if(!gameObject) return;
        
        const callbacks = GameObject.#lifeCycleCallbacks[event][key];
        if(callbacks) {
            for(const callback of callbacks) {
                callback(gameObject);
            }
        }
        if(event === "destroy") {
            gameObject.transform = null;
            delete GameObject.keyGameObjects[key];
        }
    }

    constructor(key, data) {
        this.key = key;
        this.name = data.name;
        this.transform = data.transform;
        this.hierarchyPath = data.hasOwnProperty("hierarchyPath") ? data.hierarchyPath : "";
    }

    SetActive(active) {
        this?.#invokeGameObjectEvent("gameObject.setActive", active);
    }

    InvokeMethod(methodName, paramType = "", paramValue = "") {
        paramType = Unity.types[paramType] || paramType;
        this?.#invokeGameObjectEvent("gameObject.invokeMethod", { methodName: methodName, parameterType: paramType, parameterValue: paramValue });
    }
    
    GetChild(query) {
        const eventName = typeof query === "string" ? "gameObject.findChild" : "gameObject.getChild";
        const childData = this?.#invokeGameObjectEvent(eventName, query);
        if(childData !== null) {
            const currentPath = this.hierarchyPath === "" ? this.key : this.hierarchyPath;
            childData.hierarchyPath = currentPath + "/" + childData.name;
            return new GameObject(this.key, childData);
        }
        return null;
    }
    
    Translate(x, y, z) {
        this?.#invokeGameObjectEvent("transform.translate", { x: x, y: y, z: z });
    }
    
    Rotate(x, y, z) {
        this?.#invokeGameObjectEvent("transform.rotate", { x: x, y: y, z: z });
    }
    
    SetLocalScale(x, y, z) {
        this?.#invokeGameObjectEvent("transform.setLocalScale", { x: x, y: y, z: z });
    }
    
    SetLocalPosition(x, y, z) {
        this?.#invokeGameObjectEvent("transform.setLocalPosition", { x: x, y: y, z: z });
    }
    
    SetText(text) {
        this?.#invokeGameObjectEvent("text.setText", text);
    }
    
    Destroy() {
        this?.#invokeGameObjectEvent("gameObject.destroy", "");
    }

    #invokeGameObjectEvent(eventName, payload) {
        if(!this.transform) return null;
        
        let payloadJson = payload;
        if(typeof payload === "object") payloadJson = JSON.stringify(payload);
        else if(typeof payload !== "string") payloadJson = payload.toString();
        const eventPayload = { eventName: eventName, hierarchyPath: this.hierarchyPath, payloadJson: payloadJson, listenDisabled: true };
        const response = window.Unity.InvokeEvent(`GOEvent:${this.key}`, JSON.stringify(eventPayload));
        console.log(`Invoked Event: GOEvent:${this.key}`, eventPayload);
        
        if(response === null || !response.hasOwnProperty("ok")){
            console.error(`Invalid JSON response from GameObject event callback: ${response}`);
            return null;
        }

        if(response.ok) {
            let responseObj = response.responseJson;
            try {
                responseObj = JSON.parse(response.responseJson);
            } catch {}
            return responseObj;
        }
        console.error(`Error invoking GameObject event: ${eventName}`, response.error);
        return null;
    }
}

class Unity {
    
    static GameObject = GameObject;
    static internalLogs = false;
    static types = {
        int: "System.Int32",
        float: "System.Single",
        double: "System.Double",
        bool: "System.Boolean",
        string: "System.String",
        char: "System.Char",
        byte: "System.Byte",
        long: "System.Int64",
        short: "System.Int16",
        decimal: "System.Decimal",
        object: "System.Object",
        customClass: (className, namespace = "", assembly = "Assembly-CSharp") => {
            const qualifiedName = namespace === "" ? className : `${namespace}.${className}`;
            return `${qualifiedName}, ${assembly}.dll`;
        },
    };

    static #clientEventCallback;
    static #instanceReady = false;
    static #onInstanceReadyListeners = new Set();
    static #onEventListeners = {};
    
    static LoadInstance(url, elementId) {
        Unity.#instanceReady = false;
        const r = new XMLHttpRequest();
        r.open("GET", url + "/index.html", true);
        r.onreadystatechange = function () {
            if (r.readyState !== 4 || r.status !== 200) return;
            document.querySelector(`#${elementId}`).innerHTML = r.responseText;

            const link = document.createElement('link');
            
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = url + "/TemplateData/style.css";
            document.head.appendChild(link);

            const indexScript = document.createElement("script");
            indexScript.src = url + "/index.js";
            document.body.appendChild(indexScript);
        };
        r.send();
    }

    static GetBuildVersion() {
        return Unity.InvokeEvent("InstanceEvent:GetBuildVersion");
    }
    
    static InvokeEvent(eventName, payload = undefined) {
        const responseJson = Unity.#invokeEventInternal(eventName, payload);
        try {
            const response = JSON.parse(responseJson);
            if(response.hasOwnProperty("promiseId")){
                console.warn(`Event '${eventName}' returned a promise. Consider using InvokeEventAsync instead.`);
                Unity.onEvent(`PromiseResolvedEvent:${response.promiseId}`, payload => {
                    delete Unity.#onEventListeners[`PromiseResolvedEvent:${response.promiseId}`];
                });
            }
            return response;
        }
        catch {
            return responseJson;
        }
    }
    
    static async InvokeEventAsync(eventName, payload = undefined) {
        return new Promise(resolve => {
            const responseJson = Unity.#invokeEventInternal(eventName, payload);
            try {
                const response = JSON.parse(responseJson);
                if(response.hasOwnProperty("promiseId")){
                    Unity.onEvent(`PromiseResolvedEvent:${response.promiseId}`, payload => {
                        resolve(payload);
                        delete Unity.#onEventListeners[`PromiseResolvedEvent:${response.promiseId}`];
                    });
                }
                else {
                    resolve(response);
                }
            } catch {
                resolve(responseJson);
            }
        })
    }

    static async WaitForEndOfFrameAsync() {
        return new Promise((resolve) => {
            const eventId = crypto.randomUUID().toString();
            const eventName = `EndOfFrameEvent:${eventId}`;

            Unity.onEvent(eventName, () => {
                resolve();
                delete Unity.#onEventListeners[eventName];
            });

            Unity.InvokeEvent("InstanceEvent:WaitForEndOfFrame", eventId);
        });
    }
    
    static async LoadBundleAsync(bundleUrl) {
        await Unity.InvokeEventAsync("InstanceEvent:LoadBundle", bundleUrl);
    }
    
    static async InstantiatePrefabFromBundleAsync(bundleUrl, prefabName, parentKey = "") {
        await Unity.InvokeEventAsync("InstanceEvent:InstantiatePrefabFromBundle", {
            bundleUrl: bundleUrl,
            prefabName: prefabName,
            parentKey: parentKey
        });
    }
    
    // Listeners -----------------------------
    
    static onInstanceReady(callback) {
        if(!Unity.#instanceReady) {
            Unity.#onInstanceReadyListeners.add(callback);
        }
        else {
            callback();
        }        
    }
    
    static onEvent(eventName, callback) {
        if(!Unity.#onEventListeners.hasOwnProperty(eventName)) {
            Unity.#onEventListeners[eventName] = new Set();
        }
        Unity.#onEventListeners[eventName].add(callback);
    }
    
    static offEvent(eventName, callback) {
        if(!Unity.#onEventListeners.hasOwnProperty(eventName)) return;
        Unity.#onEventListeners[eventName].delete(callback);
    }
    
    static #invokeEventInternal(eventName, payload) {
        if(payload === undefined || payload === null) payload = "";
        let payloadJson = payload;
        if(typeof payload === "object") payloadJson = JSON.stringify(payload);
        else if(typeof payload !== "string") payloadJson = payload.toString();
        const responseJson = Unity.#clientEventCallback(eventName, payloadJson);
        Unity.#onEventListeners[eventName]?.forEach(callback => callback(payloadJson));
        return responseJson;
    }

    // JSLib usage -----------------------------
    
    static _instanceReady() {
        Unity.#instanceReady = true;
        Unity.#onInstanceReadyListeners.forEach(callback => callback());
    }

    static _registerClientListener(callback) {
        Unity.#clientEventCallback = callback;
    }
    
    static _receiveEvent(eventName, payloadJson) {
        let payload = payloadJson;
        try {
            payload = JSON.parse(payloadJson);
        } catch {}

        Unity.InvokeEvent(eventName, payload);
    }
    
    static _logFromUnity(verbosity, message) {
        if(verbosity === "internal" && !Unity.internalLogs) return;
        if(verbosity === "error") console.error(`[Unity] ${message}`);
        else if(verbosity === "warning") console.warn(`[Unity] ${message}`);
        else console.log(`[Unity] ${message}`);
    }

}

window.Unity = Unity;
```

7. Import your script in the html page that will load your game:
```js
<script src="js/unity/unityInterop.js"></script>
```

8. From any other JavaScript file call `Unity.LoadInstance("url", "elementId");` where the url can be either an external url or the folder inside `wwwroot` where your build is located, and `elementId` is the name of the div where you want to contain your game canvas.
9. Use the API to control Unity.
