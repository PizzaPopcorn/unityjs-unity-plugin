# UniJS  - WebGL Interop
This library will let you interact with your Unity WebGL build from JS in a more efficient way than using SendMessage. It also includes a JavaScript library for using all available unity functions.

If you are developing a web game that can't run in editor because it depends on browser stuff or you simply want to prototype fast directly in the browser, this plugin is for you.

## Setup
1. Install this repo as a package in your Unity project using the package manager. Open the package manager, click on the add button (+) and then "Install package from git URL". Paste the clone URL for this repository on the text field.
<img width="260" height="154" alt="image" src="https://github.com/user-attachments/assets/ffb3bd84-fc51-4269-8ed4-7f9576e05f06" />


2. From this point you can either continue with step 3 or jump to the *How to use* section using the sample scene.

3. Add a null object to your starting scene and drag the `JSInstance.cs` behavior to it.
4. Drag the `JSKeyGameObject` to each object you want to expose directly to JavaScript and set a key to identify it. You will be able to access any object down in its hierarchy from JS.
5. Recommended: To maximize the fast prototyping potencial, export all of your game content to asset bundles, that way you don't have to build the game each time, just update your bundles.
6. Build your game in WebGL and either copy it somewhere inside your wwwroot folder in your web page project, or upload it to a server.
7. In your web page project, import the JavaScript library on the html file that will load the Unity instace by adding this line:
```js
<script src="https://cdn.jsdelivr.net/npm/@pizzapopcorn/unijs/dist/unity.min.js"></script>
```

8. Create another JavaScript file and also add it to the html page, then call `Unity.LoadInstance("url", "elementId");` where the url can be either an external url or the folder inside `wwwroot` where your build is located, and `elementId` is the name of the div where you want to contain your game canvas.
9. Use the API to control Unity.

## How to use
For a quick understanding of how to use the library you can follow this guide using the sample scene. If you have already your scene setup you can skip to the step 5.
1. Import the sample scene from the package manager.
2. Open the sample scene and you should see something like the image below
<img height="225" alt="sample scene" src="https://github.com/user-attachments/assets/eb60e783-e544-40f6-8cae-3777aa0344c6" />

3. Add the sample scene as the first one in the scenes list for build.
4. Follow steps 6, 7 and 8 from the *Setup* section if you haven't done already.
5. Test your build, if everything is ok you should see the same image as in step 2.
6. Now go to your JS script and add this line:
```js
Unity.onInstanceReady(() => {
    Unity.GameObject.GetKeyGameObject("Text").SetText("UniJS is awesome!");
});
```

7. Save and hit refresh and you will see that the text now will display "UniJS is awesome!" on load.
8. The `onInstanceReady` callback helps you make sure that unity is already running and the first scene has completed loading. If you try to get a gameObject outside that callback it will probably return null, unless the whole script is loaded after the unity instance is ready.
9. There's another way of making sure an object exists and it is by using the lifecycle callbacks. Add the following line to your script:
```js
Unity.GameObject.onStart("Cube", cube => {
    cube.Rotate(0, 45, 0);
});
```

10. Now save and hit refresh and you'll see that the cube appears rotated. That's how you handle start, awake and enable functions, the object might not exist yet but this tells Unity that when the `JSKeyGameObject` with the tag "Cube" exists and calls its `Start` method, it should execute that callback. This is a good method also for objects that are instanced during runtime.
11. Now, the sphere on top of the cube doesn't have a Key, but it is a child of the cube, so we can still access it. Modify the start callback as follows:
```js
Unity.GameObject.onStart("Cube", cube => {
    cube.Rotate(0, 45, 0);
    cube.GetChild(0).SetActive(false);
});
```

12. Save and hit refresh and now the sphere is gone. You can go as deep as you want inside the object's hierarchy. The `GetChild` function works both with an index or a child name. Modify the start callback as follows:
```js
Unity.GameObject.onStart("Cube", cube => {
    cube.Rotate(0, 45, 0);
    cube.GetChild("Sphere").SetActive(false);
});
```

13. Save and hit refresh and notice that the effect is the same.
14. Explore the API in JavaScript to look for other functions. Soon I will upload API documentation.
