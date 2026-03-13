using UnityEngine;

namespace UniJS.Payloads
{
    [System.Serializable]
    [ComponentPayload(typeof(Rigidbody))]
    public class RigidbodyPayload
    {
        public float mass;
        public bool useGravity;
        public bool isKinematic;
        public Vector3Payload linearVelocity;
        public Vector3Payload angularVelocity;
        public float linearDamping;
        public float angularDamping;

        public RigidbodyPayload() { }

        public RigidbodyPayload(Rigidbody rb)
        {
            mass = rb.mass;
            useGravity = rb.useGravity;
            isKinematic = rb.isKinematic;
            linearVelocity = new Vector3Payload(rb.linearVelocity);
            angularVelocity = new Vector3Payload(rb.angularVelocity);
            linearDamping = rb.linearDamping;
            angularDamping = rb.angularDamping;
        }
    }
}