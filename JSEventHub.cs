using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityJS
{
    using Events;
    using Payloads;
    
    public static class JSEventHub
    {
        private static Dictionary<string, IJSEvent<GameObject>> _gameObjectEventsByName = new();
        
        public static void Initialize()
        {
            PopulateEventCallbacks();
        }
        
        public static ResponsePayload InvokeJSEvent(this GameObject target, string eventName, string payload)
        {
            if (_gameObjectEventsByName.TryGetValue(eventName, out var callback))
            {
                return callback.Invoke(target, payload);
            }
            return ResponsePayload.Error($"Event '{eventName}' not found.");
        }
        
        private static void PopulateEventCallbacks()
        {
            _gameObjectEventsByName.Clear();
            var assembly = typeof(JSEventHub).Assembly;

            var eventTypes = assembly.GetTypes()
                .Where(t => typeof(IJSEvent<GameObject>).IsAssignableFrom(t) && t.GetCustomAttribute<JSExposedClassAttribute>() != null && !t.IsInterface && !t.IsAbstract);

            foreach (var type in eventTypes)
            {
                try
                {
                    var eventName = type.GetCustomAttribute<JSExposedClassAttribute>().Name;
                    var instance = (IJSEvent<GameObject>)Activator.CreateInstance(type);
                    if (!_gameObjectEventsByName.ContainsKey(eventName))
                    {
                        _gameObjectEventsByName.Add(eventName, instance);
                    }
                    else
                    {
                        JSInstance.LogWarning($"Duplicate event name: {eventName}");
                    }
                }
                catch (Exception ex)
                {
                    JSInstance.LogError($"Failed to instantiate {type.Name}: {ex}");
                }
            }

        }
    }
}