using System;

namespace Z
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZarchBeanAttribute : Attribute
    {
        public string[] parameters;

        public ZarchBeanAttribute(params string[] constructorParams)
        {
            parameters = constructorParams;
        }
    }
}


