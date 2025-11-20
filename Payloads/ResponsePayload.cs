namespace UnityJS.Payloads
{
    [System.Serializable]
    public class ResponsePayload
    {
        public string responseJson;
        public bool ok;
        public string error;
        
        public static ResponsePayload Error(string error)
        {
            return new ResponsePayload
            {
                ok = false,
                error = error,
                responseJson = "<error>"
            };
        }

        public static ResponsePayload Ok(string responseJson = "<ok>")
        {
            return new ResponsePayload
            {
                ok = true,
                responseJson = responseJson
            };
        }
    }
}