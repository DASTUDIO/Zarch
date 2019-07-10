﻿using System;
using System.Reflection;

namespace Z
{
    public class ReflectHelper
    {
        public object InvokeMethod(string name, object[] param, object source, Type type)
        {
            MethodInfo method = type.GetMethod(name);

            return method.Invoke(source, param);

        }

        public Delegate GetDelegate(string name, object source, Type type, Type delegateType)
        {
            MethodInfo method = type.GetMethod(name);

            return method.CreateDelegate(delegateType, source);

        }

        public object GetField(string name, object source, Type type)
        {
            FieldInfo field = type.GetField(name);

            return field.GetValue(source);
        }

        public object GetProperty(string name, object source, Type type)
        {
            PropertyInfo property = type.GetProperty(name);

            return property.GetValue(source);
        }

        public bool SetProperty(string name, object value, object source, Type type)
        {
            try
            {
                PropertyInfo property = type.GetProperty(name);

                property.SetValue(source, value);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool SetField(string name, object value, object source, Type type)
        {
            try
            {
                FieldInfo field = type.GetField(name);

                field.SetValue(source, value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Type[] GetTypes(AssemblyType type, Type containedClass)
        {
            switch (type)
            {
                case AssemblyType.Executing:
                    return Assembly.GetExecutingAssembly().GetTypes();

                case AssemblyType.Entry:
                    return Assembly.GetEntryAssembly().GetTypes();

                case AssemblyType.Calling:
                    return Assembly.GetCallingAssembly().GetTypes();

                case AssemblyType.ByContainedClass:
                    return Assembly.GetAssembly(containedClass).GetTypes();

                default:
                    return null;
            }

        }

        public object CreateInstance(Type type, object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }

        public enum AssemblyType
        {
            Entry = 0,
            Executing = 1,
            Calling = 2,
            ByContainedClass = 3
        }

    }
}
