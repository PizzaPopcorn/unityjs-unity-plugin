using UnityEngine;
using UnityJS.InstanceTools;
using UnityJS.Payloads;

namespace UnityJS
{
    public class JSKeyGameObject : MonoBehaviour
    {
        [Tooltip("The ID used to find this object in Javascript.")] 
        [SerializeField] private string JSKey;
        
        [Tooltip("Javascript can't register disabled GameObjects. Set this to false and it will disable itself after registration but before its first frame.")]
        [SerializeField] private bool startEnabled = true;

        private void Awake()
        {
            if (string.IsNullOrEmpty(JSKey))
            {
                JSKey = name;
            }
            JSInstance.LogInternal($"Registering GameObject [{JSKey}]...");
            JSInstance.RegisterKeyGameObject(JSKey, gameObject);
            JSInstance.OnEvent<string, ResponsePayload>($"GOEvent:{JSKey}", JSOnEventCallback);
            if (!startEnabled)
            {
                gameObject.SetActive(false);
            }
            JSGameObjectEventHandler.SendGameObjectLifeCycleEvent(JSKey, "awake");
        }

        private void Start()
        {
            JSGameObjectEventHandler.SendGameObjectLifeCycleEvent(JSKey, "start");
        }

        private void OnEnable()
        {
            JSGameObjectEventHandler.SendGameObjectLifeCycleEvent(JSKey, "enable");
        }
        
        private void OnDisable()
        {
            JSGameObjectEventHandler.SendGameObjectLifeCycleEvent(JSKey, "disable");
        }
        
        private void OnDestroy()
        {
            JSGameObjectEventHandler.SendGameObjectLifeCycleEvent(JSKey, "destroy");
            
        }

        private ResponsePayload JSOnEventCallback(string payload)
        {
            JSInstance.LogInternal( $"GameObject [{JSKey}] received event: {payload}");
            var eventPayload = JsonUtility.FromJson<GameObjectEventPayload>(payload);
            if (eventPayload == null) 
                return ResponsePayload.Error("Event payload failed to parse. Check if the payload is formatted correctly.");
              
            if(!gameObject.activeInHierarchy && !eventPayload.listenDisabled)
                return ResponsePayload.Ok("<ignored>");
            
            var target = gameObject;
            if (target == null)
                return ResponsePayload.Error($"The selected GameObject '{JSKey}' is null.");
                
            if (!string.IsNullOrEmpty(eventPayload.hierarchyPath))
            {
                var paths = eventPayload.hierarchyPath.Split('/');
                for (var i = 1; i < paths.Length; i++)
                {
                    var path = paths[i];
                    if (string.IsNullOrEmpty(path)) continue;
                    target = target.transform.Find(path)?.gameObject;
                    if (target == null)
                        return ResponsePayload.Error(
                            $"GameObject with hierarchy path '{eventPayload.hierarchyPath}' not found.");
                }
            }
            return target.InvokeJSEvent(eventPayload.eventName, eventPayload.payloadJson);
        }
    }
}
