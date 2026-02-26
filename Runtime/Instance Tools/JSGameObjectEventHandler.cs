using System.Runtime.InteropServices;

namespace UnityJS.InstanceTools
{
    internal class JSGameObjectEventHandler
    {
        [DllImport("__Internal")]
        private static extern void Lib_SendGameObjectLifeCycleEvent(string key, string eventName);
        
        public static void SendGameObjectLifeCycleEvent(string key, string eventName)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(eventName))
            {
                JSInstance.LogError("Invalid key or eventName provided for life cycle event.");
                return;
            }
            
            Lib_SendGameObjectLifeCycleEvent(key, eventName);
        }
    }
}