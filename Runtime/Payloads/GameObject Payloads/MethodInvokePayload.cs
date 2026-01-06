namespace UnityJS.Payloads
{
    [System.Serializable]
    public class MethodInvokePayload
    {
        public string methodName;
        
        public string parameterType;

        public string parameterValue;
    }
}