namespace UnityJS.Payloads
{
    [System.Serializable]
    public class GameObjectEventPayload
    {
        public string eventName;
        
        public string hierarchyPath;

        public string payloadJson;

        public bool listenDisabled;
    }
}