using UnityEngine;

namespace UnityJS
{
    public class JSEventButton : MonoBehaviour
    {
        public void InvokeEvent(string eventName)
        {
            JSInstance.InvokeEvent(eventName);
        }
    }
}