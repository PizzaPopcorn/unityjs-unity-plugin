using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityJS.Payloads;

namespace UnityJS.InstanceTools
{
    public class JSClientEventHandler
    {
        private static readonly Dictionary<string, List<JSIdentifiableCallback>> CallbacksByEventName = new();
        private static bool isListening;

        [DllImport("__Internal")]
        private static extern void Lib_StartListeningToClientEvents(Func<string, string, string> callback);
        
        [DllImport("__Internal")]
        private static extern void Lib_SendEventToClient(string eventName, string payloadJson);
        
        [MonoPInvokeCallback(typeof(Func<string, string, string>))]
        private static string ClientEventCallback(string eventName, string eventPayload)
        {
            string finalResult = null;
            if (CallbacksByEventName.TryGetValue(eventName, out var callbacks))
            {
                var pendingDelete = new List<JSIdentifiableCallback>();
                foreach (var callback in callbacks)
                {
                    if(callback.TryInvoke(eventPayload, out var result))
                    {
                        finalResult = result;
                    }
                    else
                    {
                        pendingDelete.Add(callback);
                    }
                }
                foreach (var callback in pendingDelete)
                {
                    callbacks.Remove(callback);
                }
            }
            return finalResult;
        }

        public static void StartListeningToClientEvents()
        {
            if (isListening) return;
            isListening = true;
            Lib_StartListeningToClientEvents(ClientEventCallback);
        }
        
        public static void StopListeningToClientEvents()
        {
            isListening = false;
        }

        public static Guid OnClientEventReceived<TPayload, TResult>(string eventName, Func<TPayload, TResult> callback)
        {
            var internalCallback = new JSIdentifiableCallback(rawPayload =>
            {
                if (PayloadParser.TryParse<TPayload>(rawPayload, out var payload))
                {
                    var result = callback.Invoke(payload);
                    JSInstance.LogInternal("Event parsed and executed correctly.\n" + rawPayload);
                    return PayloadParser.TryStringify(result, out var resultJson) ? resultJson : "<error>";
                }
                JSInstance.LogError("Event failed to parse.\n" + rawPayload);
                return "<error>";
            });
            
            if(!CallbacksByEventName.ContainsKey(eventName))
            {
                CallbacksByEventName.Add(eventName, new List<JSIdentifiableCallback>());
            }
            CallbacksByEventName[eventName].Add(internalCallback);
            return internalCallback.Id;
        }
        
        public static void OffClientEventReceived(string eventName, Guid callbackId)
        {
            if (CallbacksByEventName.TryGetValue(eventName, out var callbacks))
            {
                callbacks.RemoveAll(callback => callback.Id == callbackId);
            }
        }
        
        public static void SendEventToClient(string eventName, object payload)
        {
            PayloadParser.TryStringify(payload, out var payloadJson);
            Lib_SendEventToClient(eventName, payloadJson);
        }
    }

    public class JSIdentifiableCallback
    {
        public Guid Id { get; } = Guid.NewGuid();
        private readonly Func<string, string> _callback;
        private readonly WeakReference _targetRef;
        
        public JSIdentifiableCallback(Func<string, string> callback)
        {
            _callback = callback;
            _targetRef = new WeakReference(_callback.Target);
        }
        
        public bool TryInvoke(string payload, out string result)
        {
            result = null;
            if (!_targetRef.IsAlive) return false;
            if (_callback == null) return false;
            result = _callback.Invoke(payload);
            return true;
        }
    }
}