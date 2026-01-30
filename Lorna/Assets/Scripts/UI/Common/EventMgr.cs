namespace FrameWork_Tools
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 简易的事件分发器
    /// </summary>
    //转为UTF8格式：
    public class EventMgr :Singleton<EventMgr>
    {
        private Dictionary<string, Action<object>> eventDict = new Dictionary<string, Action<object>>();
        public void Subscribe(string eventName, Action<object> callback)
        {
            if (!eventDict.ContainsKey(eventName))
            {
                eventDict[eventName] = callback;
            }
            else
            {
                eventDict[eventName] += callback;
            }
        }

        public void Unsubscribe(string eventName, Action<object> callback)
        {
            if (eventDict.ContainsKey(eventName))
            {
                eventDict[eventName] -= callback;
                if (eventDict[eventName] == null)
                {
                    eventDict.Remove(eventName);
                }
            }
        }

        public void Publish(string eventName, object data = null)
        {
            if (eventDict.ContainsKey(eventName))
            {
                eventDict[eventName]?.Invoke(data);
            }
        }
    }
}
