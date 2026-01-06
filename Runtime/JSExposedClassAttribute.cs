using System;

namespace UnityJS
{
    [AttributeUsage(AttributeTargets.Class,  Inherited = false)]
    public class JSExposedClassAttribute : Attribute
    {
        public string Name { get; }
        public JSExposedClassAttribute(string name) => Name = name;
    }
}