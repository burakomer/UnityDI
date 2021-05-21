using UnityEngine;

namespace DwarfEngine.Tools
{
    public static class DELogger
    {
        public static bool Enabled { get; set; }

        public static void Log(object message)
        {
            if (Enabled == false) return;
            Debug.Log(message);
        }
        
        public static void Log(object message, Object context)
        {
            if (Enabled == false) return;
            Debug.Log(message, context);
        }

        public static void LogWarning(object message)
        {
            if (Enabled == false) return;
            Debug.LogWarning(message);
        }
        
        public static void LogError(object message)
        {
            if (Enabled == false) return;
            Debug.LogError(message);
        }

        public static void LogError(string message, Object context)
        {
            if (Enabled == false) return;
            Debug.LogError(message, context);
        }
    }
}