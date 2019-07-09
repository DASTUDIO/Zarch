using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Z
{
    public class Zarch
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        Dictionary<string, Type> dataManifest = new Dictionary<string, Type>();

        Dictionary<string, Func<object[], object>> method_data = new Dictionary<string, Func<object[], object>>();


        ReflectHelper reflectHelper = new ReflectHelper();

        public static Delegate CreateDelegate(string methodKey, Type delegateType)
        {
            string[] res = objects.SplitResourceKey(methodKey);

            string sourceKey = res[0];

            string methodName = res[1];

            object source = objects[sourceKey];

            Type type = objects.dataManifest[sourceKey];

            return objects.reflectHelper.GetDelegate(methodName, source, type, delegateType);

        }

        string[] SplitResourceKey(string funcKey)
        {
            string[] res = funcKey.Split('.');

            if (res.Length < 2)
                throw new Exception();

            if (res.Length == 2)
                return res;

            string[] result = new string[2];

            result[0] = funcKey.Substring(0, (funcKey.Length - res[res.Length - 1].Length - 1));

            result[1] = res[res.Length - 1];

            return result;
        }

        public object this[string name]
        {
            get
            {
                try
                {
                    string[] res = SplitResourceKey(name);

                    string sourceName = res[0];

                    string resourceName = res[1];

                    object source = objects[sourceName];

                    Type type = dataManifest[sourceName];

                    try
                    {
                        return reflectHelper.GetField(resourceName, source, type);
                    }
                    catch
                    {
                        return reflectHelper.GetProperty(resourceName, source, type);
                    }
                }
                catch
                {
                    return data[name];
                }
            }
            set
            {
                try
                {
                    string[] res = SplitResourceKey(name);

                    string sourceName = res[0];

                    string resourceName = res[1];

                    object source = objects[sourceName];

                    Type type = dataManifest[sourceName];

                    if (!reflectHelper.SetField(resourceName, value, source, type))
                        if (!reflectHelper.SetProperty(resourceName, value, source, type))
                            throw new Exception();

                }
                catch
                {
                    data[name] = value;

                    dataManifest[name] = value.GetType();

                }
            }
        }

        public string[] tree
        {
            get
            {
                List<string> res = new List<string>(data.Keys);
                res.Sort((x, y) => string.Compare(x, y));
                return res.ToArray();
            }
        }


        static Zarch _instance;

        public static Zarch objects
        {
            get
            {
                if (_instance == null)
                    _instance = new Zarch();

                return _instance;
            }
        }

        public static _ZarchMethod methods = new _ZarchMethod();

        public static object call(string methodKey, params object[] parameters)
        {
            try
            {
                string[] res = objects.SplitResourceKey(methodKey);

                string sourceKey = res[0];

                string methodName = res[1];

                object source = objects[sourceKey];

                Type type = objects.dataManifest[sourceKey];

                return objects.reflectHelper.InvokeMethod(methodName, parameters, source, type);

            }
            catch
            {
                return objects.method_data[methodKey](parameters);
            }
        }


        static string _code;

        static bool isInited;

        public static string code
        {
            get
            {
                return _code;
            }
            set
            {
                if (!isInited)
                    init();

                _code = value;

                refresh();

                run(_code);

            }
        }


        static Dictionary<Type, string[]> temp = new Dictionary<Type, string[]>();

        public static void init()
        {
            foreach (Type type in objects.reflectHelper.GetTypes())
            {
                foreach (var attributeObj in type.GetCustomAttributes(false))
                {
                    if (attributeObj is ZarchBeanAttribute)
                    {
                        string[] parameters = ((ZarchBeanAttribute)attributeObj).parameters;

                        if (parameters == null)
                            objects[type.Name] = objects.reflectHelper.CreateInstance(type, null);
                        else
                        {
                            try
                            {
                                List<object> parameterObjects = new List<object>();

                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    parameterObjects.Add(objects[parameters[i]]);
                                }

                                objects[type.Name] = objects.reflectHelper.CreateInstance(type, parameterObjects.ToArray());
                            }
                            catch
                            {
                                temp.Add(type, parameters);
                            }
                        }
                    }
                }
            }
            refresh();
            isInited = true;
        }

        public static void refresh()
        {
            var tmp_keys = new List<Type>(temp.Keys);

            for (int i = 0; i < tmp_keys.Count; i++)
            {
                Type type = tmp_keys[i];

                string[] parameters = temp[tmp_keys[i]];

                try
                {
                    List<object> parameterObjects = new List<object>();

                    for (int j = 0; j < parameters.Length; j++)
                    {
                        parameterObjects.Add(objects[parameters[i]]);
                    }

                    objects[type.Name] = objects.reflectHelper.CreateInstance(type, parameterObjects.ToArray());

                    temp.Remove(tmp_keys[i]);
                }
                catch { }

            }
        }

        static void run(string code)
        {
            string[] lines = objects.GetLines(code);

            for (int i = 0; i < lines.Length; i++)
            {
                objects.RunOneLine(lines[i]);
            }
        }

        List<string> tmpKeys = new List<string>();

        Regex regex = new Regex(@"([^\(=]+?\([^()]*?\))");

        string GenTmpKey()
        {
            string res = "tmp_";

            Random r = new Random(System.DateTime.Now.Millisecond);

            res += r.Next(100, 1000);

            if (tmpKeys.Contains(res))
                return GenTmpKey();

            return res;
        }

        void RunOneLine(string line)
        {
            line = line.Replace(" ", "");

            if (line.Length > 0)
            {
                if (line.Substring(0, 1) == "#")
                    return;
            }
            else
                return;

            if (IsMethod(line))
                line = ProcessMethods(line);

            if (objects.IsExpression(line))
                ProcessExpression(line);

            for (int i = 0; i < tmpKeys.Count; i++)
            {
                objects.data.Remove(tmpKeys[i]);
            }
        }

        string ProcessMethods(string line)
        {
            char[] chars = line.ToArray();

            int left = 0;

            int right = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '(')
                    left++;

                if (chars[i] == ')')
                    right++;
            }

            if (left != right)
                throw new Exception("括号不对称");

            while (IsMethod(line))
            {
                MatchCollection match = regex.Matches(line);

                for (int i = 0; i < match.Count; i++)
                {
                    string phrase = match[i].Groups[1].Value;

                    string methodKey = phrase.Substring(0, phrase.IndexOf("(", StringComparison.Ordinal));

                    string paramKeys = phrase.Substring(phrase.IndexOf("(", StringComparison.Ordinal)).Replace("(", "").Replace(")", "");

                    List<object> param = new List<object>();

                    if (paramKeys.Length != 0)
                    {
                        if (paramKeys.IndexOf(",", StringComparison.Ordinal) > 0)
                        {
                            string[] pms = paramKeys.Split(',');

                            for (int j = 0; j < pms.Length; j++)
                            {
                                param.Add(objects[pms[j]]);
                            }
                        }
                        else
                            param.Add(objects[paramKeys]);
                    }

                    string tmpKey = GenTmpKey();

                    tmpKeys.Add(tmpKey);

                    data[tmpKey] = call(methodKey, param.ToArray());

                    line = line.Replace(phrase, tmpKey);
                }
            }
            return line;
        }

        void ProcessExpression(string line)
        {
            string[] res = line.Replace(" ", "").Split('=');

            for (int i = res.Length - 1; i > 0; i--)
            {
                objects[res[i - 1]] = objects[res[i]];
            }

        }

        string[] GetLines(string content)
        {
            string[] lns = content.Split(';')
                .Where(s => !string.IsNullOrEmpty(s)).ToArray();

            return lns;
        }


        bool IsExpression(string line)
        {
            var first = line.IndexOf("=", StringComparison.Ordinal);

            var last = line.LastIndexOf("=", StringComparison.Ordinal);

            if (first != -1 && last != -1 && first != 0 && last != line.Length - 1)
                return true;

            return false;

        }

        bool IsMethod(string line)
        {
            return regex.IsMatch(line);
        }

        public class _ZarchMethod
        {
            public Func<object[], object> this[string name]
            {
                get
                {
                    return (parameters) => call(name, parameters);

                }
                set
                {
                    objects.method_data[name] = value;
                }
            }

        }

    }

}