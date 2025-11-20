using System;
using UnityEngine;
using UnityJS.Payloads;

namespace UnityJS.Events
{
    [JSExposedClass("gameObject.setActive")]
    public class Event_GameObjectSetActive : JSEventVoid<GameObject, bool> 
    {
        protected override void Invoke(GameObject target, bool state)
        {
            target.SetActive(state);
        }
    }
    
    [JSExposedClass("gameObject.invokeMethod")]
    public class Event_GameObjectInvokeMethod : JSEventVoid<GameObject, MethodInvokePayload> 
    {
        protected override void Invoke(GameObject target, MethodInvokePayload payload)
        {
            if (string.IsNullOrEmpty(payload.parameterType))
            {
                target.SendMessage(payload.methodName);
                return;
            }

            var type = Type.GetType(payload.parameterType);
            if(type == null)
            {
                //TODO: Check why it's failing to parse class types
                JSInstance.LogError($"Failed to invoke method '{payload.methodName}' on GameObject '{target.name}' with parameter of type '{payload.parameterType}'. Parameter type failed to parse.");
                return;
            }
            
            if (PayloadParser.TryParse(payload.parameterValue, type, out var parameter))
            {
                target.SendMessage(payload.methodName, parameter);
            }
            else
            {
                JSInstance.LogError($"Failed to invoke method '{payload.methodName}' on GameObject '{target.name}' with parameter '{payload.parameterValue}' of type '{payload.parameterType}'. Parameter value failed to parse.");
            }
        }
    }
    
    [JSExposedClass("gameObject.getChild")]
    public class Event_GameObjectGetChild : JSEvent<GameObject, int, JSGameObjectData>
    {
        protected override JSGameObjectData Invoke(GameObject target, int childIndex)
        {
            var child = target.transform.GetChild(childIndex);
            return child == null ? null : new JSGameObjectData(child.gameObject);
        }
    }
    
    [JSExposedClass("gameObject.findChild")]
    public class Event_GameObjectFindChild : JSEvent<GameObject, string, JSGameObjectData>
    {
        protected override JSGameObjectData Invoke(GameObject target, string name)
        {
            var child = target.transform.Find(name);
            return child == null ? null : new JSGameObjectData(child.gameObject);
        }
    }
}