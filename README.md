# UnityJS
This library will let you interact with your Unity WebGL build from JS in a more efficient way than using SendMessage. It also includes a JavaScript library for using all available unity functions.

If you are developing a web game that can't run in editor because it depends on browser stuff or you simply want to prototype fast directly in the browser, this plugin is for you.

## How to use
1. Install this repo as a package in your Unity project using the package manager.
2. Add a null object to your starting scene and drag the `JSInstance.cs` behavior to it.
3. Drag the `JSKeyGameObject` to each object you want to expose directly to JavaScript and set a key to identify it. You will be able to access any object down in its hierarchy from JS.
4. Recommended: To maximize the fast prototyping potencial, export all of your game content to asset bundles, that way you don't have to build the game each time, just update your bundles.
5. Build your game in WebGL
6. Import the Javascript library to your web page project in the html file that will load the Unity instace by adding this line:
```js
<script src="https://cdn.jsdelivr.net/npm/@pizzapopcorn/unityjs/dist/unity.min.js"></script>
```

7. From any other JavaScript file call `Unity.LoadInstance("url", "elementId");` where the url can be either an external url or the folder inside `wwwroot` where your build is located, and `elementId` is the name of the div where you want to contain your game canvas.
8. Use the API to control Unity.
