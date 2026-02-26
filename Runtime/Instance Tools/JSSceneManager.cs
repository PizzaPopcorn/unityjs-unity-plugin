using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityJS.InstanceTools
{
    internal class JSSceneManager
    {
        private static AsyncOperation _sceneLoadOperation;
        
        public static void LoadSceneAsync(string sceneName, Action onSceneLoaded, Action<string> onFailed)
        {
            _sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
            if (_sceneLoadOperation == null)
            {
                onFailed?.Invoke($"Failed to load scene {sceneName}");
                return;
            }
            _sceneLoadOperation.completed += HandleSceneLoadOperationCompleted;
            return;

            void HandleSceneLoadOperationCompleted(AsyncOperation async)
            {
                if (async != _sceneLoadOperation) return;
                
                onSceneLoaded?.Invoke();
                _sceneLoadOperation = null;
            }
        }

        public static bool IsSceneLoading()
        {
            return _sceneLoadOperation != null;
        }

        public static float GetSceneLoadProgress()
        {
            return _sceneLoadOperation?.progress ?? 0;
        }
    }
}