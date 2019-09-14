using System;
using System.Text;
using System.Collections.Generic;

namespace Z
{
    public class ZarchFunctions
    {

        public Action<string> printEvent;

        // int
        public object GetInt(object value)
        {
            if (value is String)
                return Convert.ToInt32(Int32.Parse((String)value));
            return (int)Convert.ToInt32(value);
        }

        // float
        public object GetFloat(object value)
        {
            if (value is String)
                return Convert.ToDouble(float.Parse((String)value));
            return (float)Convert.ToDouble(value);
        }

        // double
        public object GetDouble(object value)
        {
            if (value is String)
                return Convert.ToDouble(double.Parse((String)value));
            return (double)Convert.ToDouble(value);
        }

        // print print(a,b,c,d)
        public object Print(params object[] parameters)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item in parameters)
            {
                builder.Append(item);
            }
            Console.WriteLine(builder);

            if (printEvent != null)
                printEvent(builder.ToString());

            return null;
        }

        // str str(a,b,c,d)
        public object GetString(object[] parameters)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item in parameters)
            {
                builder.Append(item.ToString());
            }

            return builder.ToString();
        }

        //if if(bool(x,y),fun1,fun2);
        public object DoIf(object[] parameters)
        {
            object[] param = new object[parameters.Length - 3];

            Array.ConstrainedCopy(parameters, 3, param, 0, param.Length);

            if ((bool)parameters[0])
                return ((Func<object[], object>)parameters[1])(param);
            else
                return ((Func<object[], object>)parameters[2])(param);
        }

        // bool bool(x,y) bool(1) bool('true')
        public object GetBool(object[] parameters)
        {
            switch (parameters.Length)
            {
                case 0:
                    return false;
                case 1:
                    if (parameters[0] is bool)
                        return (bool)parameters[0];
                    if (parameters[0] is string)
                        return (((string)parameters[0]).ToLower().Replace(" ", "") == "true");

                    if (parameters[0] is int)
                        return (((int)parameters[0]) > 0);

                    if (parameters[0] is float)
                        return ((float)parameters[0] > 0);

                    if (parameters[0] is double)
                        return ((double)parameters[0] > 0);

                    if (parameters[0] != null)
                        return true;

                    return false;
                case 2:
                    if (parameters[0] is bool && parameters[1] is bool)
                        return ((bool)parameters[0] == (bool)parameters[1]);

                    if (parameters[0] is string && parameters[1] is string)
                        return ((string)parameters[0] == (string)parameters[1]);

                    if (parameters[0] is int && parameters[1] is int)
                        return ((int)parameters[0] == (int)parameters[1]);

                    if (parameters[0] is float && parameters[1] is float)
                        return ((float)parameters[0] == (float)parameters[1]);


                    if (parameters[0] is double && parameters[1] is double)
                        return ((double)parameters[0] == (double)parameters[1]);


                    return false;

                default:
                    return false;
            }
        }

        // list
        public object GetList(object[] parameters)
        {
            List<object> res = new List<object>();
            for (int i = 0; i < parameters.Length; i++)
            {
                res.Add(parameters[i]);
            }
            return res;
        }

        // for(0,100,15,func)
        public object DoFor(object[] parameters)
        {
            int start = (int)parameters[0];
            int step = (int)parameters[1];
            int end = (int)parameters[2];
            Func<object[], object> func = (Func<object[], object>)parameters[3];
            object[] param = new object[parameters.Length - 4];
            Array.ConstrainedCopy(parameters, 4, param, 0, param.Length);

            if (start < end)
                for (int i = start; i < end; i = i + step)
                { func(param); }
            if (start > end)
                for (int i = end; i < start; i = i + step)
                { func(param); }

            return null;

        }


        public object GetNull(object[] parameters)
        {
            return null;
        }

    }

}

