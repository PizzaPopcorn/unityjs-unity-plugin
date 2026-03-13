using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UniJS.Payloads
{
    public static class ComponentPayloadConverter
    {
        private static readonly Dictionary<Type, Type> PayloadTypesByComponentType = new();

        static ComponentPayloadConverter()
        {
            var assembly = typeof(ComponentPayloadConverter).Assembly;
            var payloadTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ComponentPayloadAttribute>() != null);

            foreach (var payloadType in payloadTypes)
            {
                var attr = payloadType.GetCustomAttribute<ComponentPayloadAttribute>();
                PayloadTypesByComponentType[attr.ComponentType] = payloadType;
            }
        }

        public static object Convert(Component component)
        {
            if (component == null) return null;

            var componentType = component.GetType();
            
            // Try exact match or base class matches
            foreach (var kvp in PayloadTypesByComponentType)
            {
                if (kvp.Key.IsAssignableFrom(componentType))
                {
                    return Activator.CreateInstance(kvp.Value, component);
                }
            }

            return null;
        }
    }
}