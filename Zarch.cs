using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Z
{
    public class Zarch
    {
        #region Elements

        Dictionary<string, object> data = new Dictionary<string, object>();

        Dictionary<string, Type> dataManifest = new Dictionary<string, Type>();

        Dictionary<string, Type> class_data = new Dictionary<string, Type>();

        Dictionary<string, Func<object[], object>> method_data = new Dictionary<string, Func<object[], object>>();

        #endregion

        #region Facade

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

        public static Zarch objects
        {
            get
            {
                if (_instance == null)
                    _instance = new Zarch();

                return _instance;
            }
        }

        public static _ZarchClass classes = new _ZarchClass();

        public static _ZarchMethod methods = new _ZarchMethod();

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
                try
                {
                    return ((Func<object[], object>)objects[methodKey])(parameters);

                }
                catch
                {
                    return objects.method_data[methodKey](parameters);
                }
            }

        }

        public static Delegate CreateDelegate(string methodKey, Type delegateType)
        {
            string[] res = objects.SplitResourceKey(methodKey);

            string sourceKey = res[0];

            string methodName = res[1];

            object source = objects[sourceKey];

            Type type = objects.dataManifest[sourceKey];

            return objects.reflectHelper.GetDelegate(methodName, source, type, delegateType);

        }


        public static void init()
        {
            foreach (Type type in objects.reflectHelper.GetTypes(ReflectConfig.Assembly, ReflectConfig.ContainedClass))
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

            foreach (var item in FunctionConfigs)
            {
                objects.method_data[item.Key] = item.Value;
            }

            isInited = true;

            refresh();
        }

        public static void refresh()
        {
            // Dependence
            if (temp.Count == 0)
                return;

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

        public static void clear()
        {
            objects.data.Clear();
            objects.method_data.Clear();
            objects.class_data.Clear();
            objects.dataManifest.Clear();
            objects.tmpKeys.Clear();
            isInited = false;
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


        public static Action<string> LogDelegate
        { set { objects.functions.printEvent = value; } }

        public static Dictionary<string, Func<object[], object>> FunctionConfigs = new Dictionary<string, Func<object[], object>>
        {
            { "int",objects.functions.GetInt },
            { "float",objects.functions.GetFloat },
            { "double",objects.functions.GetDouble },
            { "str", objects.functions.GetString },
            { "list", objects.functions.GetList },
            { "null", objects.functions.GetNull },
            { "bool", objects.functions.GetBool },
            { "if", objects.functions.DoIf },
            { "for",objects.functions.DoFor },
            { "print",objects.functions.Print }
        };


        #endregion

        #region Internal

        #region Internal Elements

        static Dictionary<Type, string[]> temp = new Dictionary<Type, string[]>();

        ZarchFunctions functions = new ZarchFunctions();

        ZarchReflectHelper reflectHelper = new ZarchReflectHelper();

        static Zarch _instance;

        static string _code;

        static bool isInited;

        #endregion

        #region phrase analyze

        static void run(string zarchCode)
        {
            if (objects.codeBlockBody.IsMatch(zarchCode))
                zarchCode = objects.ProcessCodeBlock(zarchCode);

            string[] lines = objects.GetLines(zarchCode);

            for (int i = 0; i < lines.Length; i++)
            {
                objects.RunOneLine(lines[i]);
            }
        }

        void RunOneLine(string line)
        {
            line = line.Replace(" ", "");

            if (line.Length < 1)
                return;

            if (line.Substring(0, 1) == "#")
                return;


            if (IsMethod(line))
                line = ProcessMethods(line);

            if (IsExpression(line))
                ProcessExpression(line);

            for (int i = 0; i < objects.tmpKeys.Count; i++)
            {
                objects.data.Remove(objects.tmpKeys[i]);
            }
        }

        string[] GetLines(string content)
        {
            string[] lns = content.Split(';')
                .Where(s => !string.IsNullOrEmpty(s)).ToArray();

            return lns;
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

        string GenTmpKey()
        {
            string res = "tmp_";

            Random r = new Random();

            res += r.Next();

            if (tmpKeys.Contains(res))
                return GenTmpKey();

            return res;
        }

        #region Method

        Regex methodBody = new Regex(@"([^\(=]+?\([^()]*?\))");

        string commaPlaceholder = "@#&comma&#@";

        List<string> tmpKeys = new List<string>();

        bool IsMethod(string line)
        {
            return methodBody.IsMatch(line);
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

            // 从左往右
            while (IsMethod(line))
            {
                MatchCollection match = methodBody.Matches(line);

                if (match.Count == 0)
                    break;

                string phrase = match[0].Groups[1].Value;

                string methodKey = phrase.Substring(0, phrase.IndexOf("(", StringComparison.Ordinal));

                string paramKeys = phrase.Substring(phrase.IndexOf("(", StringComparison.Ordinal)).Replace("(", "").Replace(")", "");

                List<object> param = new List<object>();

                if (paramKeys.Length != 0)
                {
                    if (paramKeys.IndexOf(",", StringComparison.Ordinal) > 0)
                    {
                        string[] strs = paramKeys.Split('\'');

                        StringBuilder builder = new StringBuilder();

                        for (int i = 1; i < strs.Length; i = i + 2)
                        {
                            strs[i] = strs[i].Replace(",", commaPlaceholder);
                        }

                        for (int i = 0; i < strs.Length; i++)
                        {
                            if (i == 0)
                                builder.Append(strs[i]);
                            else
                                builder.Append("'").Append(strs[i]);
                        }

                        paramKeys = builder.ToString();

                        string[] pms = paramKeys.Split(',');

                        for (int j = 0; j < pms.Length; j++)
                        {
                            pms[j] = pms[j].Replace(commaPlaceholder, ",");

                            if (isString.IsMatch(pms[j]))
                            {
                                param.Add(isString.Match(pms[j]).Groups[1].Value);
                            }
                            else if (isInt.IsMatch(pms[j]))
                            {
                                param.Add(Int32.Parse(isInt.Match(pms[j]).Groups[1].Value));
                            }
                            else if (isDouble.IsMatch(pms[j]))
                            {
                                param.Add(Double.Parse(isDouble.Match(pms[j]).Groups[1].Value));
                            }
                            else if (isDelegate.IsMatch(pms[j]))
                            {
                                string delegateKey = isDelegate.Match(pms[j]).Groups[1].Value;
                                param.Add(methods[delegateKey]);
                            }
                            else
                            {
                                param.Add(objects[pms[j]]);
                            }
                        }
                    }
                    else
                    {
                        if (isString.IsMatch(paramKeys))
                        {
                            param.Add(isString.Match(paramKeys).Groups[1].Value);
                        }
                        else if (isInt.IsMatch(paramKeys))
                        {
                            param.Add(Int32.Parse(isInt.Match(paramKeys).Groups[1].Value));
                        }
                        else if (isDouble.IsMatch(paramKeys))
                        {
                            param.Add(Double.Parse(isDouble.Match(paramKeys).Groups[1].Value));
                        }
                        else if (isDelegate.IsMatch(paramKeys))
                        {
                            string delegateKey = isDelegate.Match(paramKeys).Groups[1].Value;
                            param.Add(methods[delegateKey]);
                        }
                        else
                        {
                            param.Add(objects[paramKeys]);
                        }

                    }
                }

                string tmpKey = GenTmpKey();

                tmpKeys.Add(tmpKey);

                try
                {
                    dataManifest[tmpKey] = classes[methodKey];
                    data[tmpKey] = classes.GetConstrutor(methodKey)(param.ToArray());
                }
                catch
                {
                    data[tmpKey] = methods[methodKey](param.ToArray());
                }

                line = line.Replace(phrase, tmpKey);

            }
            return line;
        }

        #endregion

        #region Express

        Regex isString = new Regex(@"^'([^']*?)'$");

        Regex isInt = new Regex(@"(^\d+?$)");

        Regex isDouble = new Regex(@"(^\d+?\.\d+?$)");

        Regex isDelegate = new Regex(@"^\[([^\[\]\(\)\{\}\=]+?)\]$");

        bool IsExpression(string line)
        {
            var first = line.IndexOf("=", StringComparison.Ordinal);

            var last = line.LastIndexOf("=", StringComparison.Ordinal);

            if (first != -1 && last != -1 && first != 0 && last != line.Length - 1)
                return true;

            return false;

        }

        void ProcessExpression(string line)
        {
            string[] res = line.Replace(" ", "").Split('=');

            for (int i = res.Length - 1; i > 0; i--)
            {
                if (isString.IsMatch(res[i]))
                {
                    objects[res[i - 1]] = isString.Match(res[i]).Groups[1].Value;
                }
                else if (isInt.IsMatch(res[i]))
                {
                    objects[res[i - 1]] = Int32.Parse(isInt.Match(res[i]).Groups[1].Value);
                }
                else if (isDouble.IsMatch(res[i]))
                {
                    objects[res[i - 1]] = Double.Parse(isDouble.Match(res[i]).Groups[1].Value);
                }
                else if (isDelegate.IsMatch(res[i]))
                {
                    string delegateKey = isDelegate.Match(res[i]).Groups[1].Value;
                    objects[res[i - 1]] = methods[delegateKey];
                }
                else
                {
                    objects[res[i - 1]] = objects[res[i]];
                }
            }
        }

        #endregion

        #region code block

        Regex codeBlockBody = new Regex(@"(\{[^{}]*?\})");

        Regex codeBlockSection = new Regex(@"\{([^{}]*?)\}");

        public string ProcessCodeBlock(string line)
        {
            MatchCollection matches = codeBlockBody.Matches(line);

            for (int i = 0; i < matches.Count; i++)
            {
                string codeBodyStr = matches[i].Groups[1].Value;

                string codeSectionStr = codeBlockSection.Match(codeBodyStr).Groups[1].Value;

                string tmp_key = GenTmpKey();

                tmpKeys.Add(tmp_key);

                method_data.Add(tmp_key, (param) => { run(codeSectionStr); return null; });

                line = line.Replace(codeBodyStr, "[" + tmp_key + "]");

            }

            return line;
        }

        #endregion

        #endregion

        #region internal classes

        public class ReflectConfig
        {
            public static ZarchReflectHelper.AssemblyType Assembly = ZarchReflectHelper.AssemblyType.Entry;

            public static Type ContainedClass = typeof(Zarch);
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
                    if (FunctionConfigs.ContainsKey(name))
                        throw new Exception("该函数名属于内置功能" + name);
                    objects.method_data[name] = value;
                }
            }

        }

        public class _ZarchClass
        {
            public Type this[string name]
            {
                get
                {
                    return objects.class_data[name];
                }

                set
                {
                    objects.class_data[name] = value;
                }
            }

            public Func<object[], object> GetConstrutor(string name)
            {
                return (object[] param) => { return objects.reflectHelper.CreateInstance(objects.class_data[name], param); };
            }

        }

        #endregion

        #endregion



    }
}
