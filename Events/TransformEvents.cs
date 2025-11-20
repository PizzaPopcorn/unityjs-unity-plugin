using UnityJS.Payloads;
using UnityEngine;

namespace UnityJS.Events
{
    [JSExposedClass("transform.translate")]
    public class Event_TransformTranslate : JSEvent<GameObject, Vector3Payload, Vector3Payload>
    {
        protected override Vector3Payload Invoke(GameObject target, Vector3Payload vector)
        {
            target.transform.Translate(vector.x, vector.y, vector.z);
            var newPos = target.transform.position;
            return new Vector3Payload { x = newPos.x, y = newPos.y, z = newPos.z };
        }
    }

    [JSExposedClass("transform.rotate")]   
    public class Event_TransformRotate : JSEvent<GameObject, Vector3Payload, Vector3Payload>
    {
        protected override Vector3Payload Invoke(GameObject target, Vector3Payload euler)
        {
            target.transform.Rotate(euler.x, euler.y, euler.z);
            var newRot = target.transform.eulerAngles;
            return new Vector3Payload { x = newRot.x, y = newRot.y, z = newRot.z };
        }
    }
    
    [JSExposedClass("transform.setLocalScale")]  
    public class Event_TransformSetLocalScale : JSEventVoid<GameObject, Vector3Payload>
    {
        protected override void Invoke(GameObject target, Vector3Payload scale)
        {
            target.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }
    
    [JSExposedClass("transform.setLocalPosition")] 
    public class Event_TransformSetLocalPosition : JSEventVoid<GameObject, Vector3Payload>
    {
        protected override void Invoke(GameObject target, Vector3Payload position)
        {
            target.transform.localPosition = new Vector3(position.x, position.y, position.z);
        }
    }
}