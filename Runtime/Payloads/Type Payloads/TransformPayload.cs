
namespace UnityJS.Payloads
{
    [System.Serializable]
    public class TransformPayload
    {
        public Vector3Payload position;
        
        public QuaternionPayload rotation;
        
        public Vector3Payload scale;
    }
}