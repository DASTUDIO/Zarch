using System;
using System.Reflection;

namespace Z
{

    public class ZarchReflectHelper
    {
        public object InvokeMethod(string name, object[] param, object source, Type type)
        {
            // 静态方法
            if (source == null)
            {
                MethodInfo[] _methods = type.GetMethods();

                foreach (var _method in _methods) {
                    if (_method.Name != name)
                        continue;

                    try
                    {
                        return _method.Invoke(null, param);
                    }
                    catch { }

                }

                foreach (var _method in _methods)
                {
                    if (_method.Name != name)
                        continue;

                    try
                    {
                        return _method.Invoke(null, null);
                    }
                    catch { }

                }

                throw new Exception();
            }

            MethodInfo[] methods = type.GetMethods();

            // 为了重载  // 有参成员
            foreach (var method in methods)
            {
                if (method.Name != name)
                    continue;

                try
                {
                    return method.Invoke(source, param);
                }
                catch { }
            }

            // 无参成员
            foreach (var method in methods)
            {
                if (method.Name != name)
                    continue;

                try
                {
                    return method.Invoke(source, null);
                }
                catch { }

            }

            throw new Exception();
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

        public Type[] GetTypes(AssemblyType asmType, Type containedClass = null)
        {
            switch (asmType)
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

        public Type GetType(AssemblyType asmType ,string name)
        {
            Type[] tps = GetTypes(asmType,null) ;

            foreach (var item in tps)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            switch (asmType)
            {
                case AssemblyType.Executing:
                    return Assembly.GetExecutingAssembly().GetType(name);

                case AssemblyType.Entry:
                    return Assembly.GetEntryAssembly().GetType(name);

                case AssemblyType.Calling:
                    return Assembly.GetCallingAssembly().GetType(name);

                default:
                    return null;
            }
        }

        public object CreateInstance(Type type, object[] parameters)
        {
            try
            {
                return Activator.CreateInstance(type, parameters);
            }
            catch
            {
                return Activator.CreateInstance(type);
            }
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
