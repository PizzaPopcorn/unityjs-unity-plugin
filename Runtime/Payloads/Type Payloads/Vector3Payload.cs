
using UnityEngine;

namespace UniJS.Payloads
{
    [System.Serializable]
    public class Vector3Payload
    {
        public float x;
        public float y;
        public float z;

        public Vector3Payload() { }

        public Vector3Payload(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
    }
}