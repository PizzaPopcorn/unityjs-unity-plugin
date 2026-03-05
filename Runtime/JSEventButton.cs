using UnityEngine;

namespace UniJS
{
    public class JSEventButton : MonoBehaviour
    {
        public void InvokeEvent(string eventName)
        {
            JSInstance.InvokeEvent(eventName);
        }
    }
}