namespace LornaGame.ModuleExtensions
{
    using UnityEngine;
    using Debug = UnityEngine.Debug;


    public interface ILogger
    {
        void Log(string msg, string stack, LogType type);
    }

    public static class Logs
    {
        public static bool useLog = true;
        public static string threadStack = string.Empty;
        public static ILogger logger = (ILogger)null;

        //[Conditional("DEBUGLOG")]
        private static void LogInfo(string str)
        {
            if (Logs.useLog)
            {
                Debug.Log((object)str);
            }
            else
            {
                if (Logs.logger == null)
                    return;
                Logs.logger.Log(str, string.Empty, LogType.Log);
            }
        }

        //[Conditional("DEBUGLOG")]
        public static void Log(string str)
        {
            Logs.LogInfo(str.WhiteColor());
        }

        //[Conditional("DEBUGLOG")]
        public static void Log(string str, object arg0)
        {
            Logs.Log(string.Format(str, arg0));
        }

        //[Conditional("DEBUGLOG")]
        public static void Log(string str, object arg0, object arg1)
        {
            Logs.Log(string.Format(str, arg0, arg1));
        }

        //[Conditional("DEBUGLOG")]
        public static void Log(string str, object arg0, object arg1, object arg2)
        {
            Logs.Log(string.Format(str, arg0, arg1, arg2));
        }

        //[Conditional("DEBUGLOG")]
        public static void Log(string str, params object[] param)
        {
            Logs.Log(string.Format(str, param));
        }

        //[Conditional("DEBUGLOG")]
        public static void LogWarning(string str)
        {
            Logs.LogInfo(str.YellowColor());
        }

        //[Conditional("DEBUGLOG")]
        public static void LogWarning(object message)
        {
            Logs.LogWarning(message.ToString());
        }

        //[Conditional("DEBUGLOG")]
        public static void LogWarning(string str, object arg0)
        {
            Logs.LogWarning(string.Format(str, arg0));
        }

        //[Conditional("DEBUGLOG")]
        public static void LogWarning(string str, params object[] param)
        {
            Logs.LogWarning(string.Format(str, param));
        }

        //[Conditional("DEBUGLOG")]
        public static void LogError(string str)
        {
            UnityEngine.Debug.LogError(str);    
           // Logs.LogInfo(str.RedColor());
        }

        //[Conditional("DEBUGLOG")]
        public static void LogError(object message)
        {
            Logs.LogError(message.ToString());
        }

        //[Conditional("DEBUGLOG")]
        public static void LogError(string str, object arg0)
        {
            Logs.LogError(string.Format(str, arg0));
        }

        //[Conditional("DEBUGLOG")]
        public static void LogError(string str, object arg0, object arg1)
        {
            Logs.LogError(string.Format(str, arg0, arg1));
        }

        //[Conditional("DEBUGLOG")]
        public static void LogError(string str, object arg0, object arg1, object arg2)
        {
            Logs.LogError(string.Format(str, arg0, arg1, arg2));
        }

        //[Conditional("DEBUGLOG")]
        public static void LogError(string str, params object[] param)
        {
            Logs.LogError(string.Format(str, param));
        }
    }
}