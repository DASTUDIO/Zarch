using System;
using System.Text;
using System.Threading;

namespace Z.Tools
{
    public class Logger
    {
        #region Elements

        /// <summary>
        /// 打印类型 控制台 or Unity
        /// </summary>
        public static LogType logType = LogType.Console;

        /// <summary>
        /// 是否开启打印
        /// </summary>
        public static bool DoLog = true;

        #endregion

        /// <summary>
        /// 普通样式 Log 打印一条信息
        /// </summary>
        /// <returns>The log.</returns>
        /// <param name="msg">Message.</param>
        public static void Log(string msg)
        {
            msg = DateTime.Now.ToString() + " " + msg + "\n";

           
            if (DoLog)
            {
                switch (logType)
                {
                    case LogType.Console:
                        Console.WriteLine(msg);
                        break;
                    case LogType.Custom:
                        if (Zarch.LogDelegate != null)
                            Zarch.LogDelegate(msg);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 警告样式 Log 打印一条信息
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void LogWarnning(string msg)
        {
            msg = "[WARNING] " + DateTime.Now.ToString() + " " + msg + "\n";

            if (DoLog)
            {
                switch (logType)
                {
                    case LogType.Console:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(msg);
                        Console.ResetColor();
                        break;
                    case LogType.Custom:
                        if (Zarch.LogDelegate != null)
                            Zarch.LogDelegate(msg);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 错误样式 Log 打印一条信息
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void LogError(string msg)
        {
            msg = "[ERROR] " + DateTime.Now.ToString() + " " + msg + "\n";

            
            if (DoLog)
            {
                switch (logType)
                {
                    case LogType.Console:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(msg);
                        Console.ResetColor();
                        break;
                    case LogType.Custom:
                        if (Zarch.LogDelegate != null)
                            Zarch.LogDelegate(msg);
                        break;
                    default:
                        break;
                }

            }
        }


        static bool FileLoggerIsRunning = false;


        private Logger() { }

        public enum LogType : Byte
        {
            Console = 0,
            Custom = 1
        }


    }
}
