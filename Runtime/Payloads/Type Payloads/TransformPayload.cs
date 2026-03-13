
using UnityEngine;

namespace UniJS.Payloads
{
    [System.Serializable]
    [ComponentPayload(typeof(Transform))]
    public class TransformPayload
    {
        public Vector3Payload position;
        public QuaternionPayload rotation;
        public Vector3Payload scale;

        public TransformPayload() { }

        public TransformPayload(Transform transform)
        {
            position = new Vector3Payload(transform.position);
            rotation = new QuaternionPayload(transform.rotation);
            scale = new Vector3Payload(transform.localScale);
        }
    }
}