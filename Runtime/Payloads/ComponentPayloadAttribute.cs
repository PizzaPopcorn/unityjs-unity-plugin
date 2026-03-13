using System;

namespace UniJS.Payloads
{
    /// <summary>
    /// Attribute used to link a Unity Component to its Payload class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ComponentPayloadAttribute : Attribute
    {
        public Type ComponentType { get; }

        public ComponentPayloadAttribute(Type componentType)
        {
            ComponentType = componentType;
        }
    }
}
