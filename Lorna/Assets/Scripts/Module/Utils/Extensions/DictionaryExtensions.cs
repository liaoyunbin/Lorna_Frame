using LornaGame.ModuleExtensions;
using System;
using System.Collections.Generic;

namespace LornaGame.ModuleExtensions{
    public static class DictionaryExtensions{
        public static TV GetValue<TK, TV>(this Dictionary<TK, TV> dic, TK key){
            TV value = default(TV);
            dic.TryGetValue(key, out value);
            return value;
        }

        public static TK GetKey<TK, TV>(this Dictionary<TK, TV> dic, TV value){
            TK key = default(TK);
            if (value == null){
                return key;
            }

            foreach (var data in dic){
                if (data.Value.Equals(value)){
                    key = data.Key;
                }
            }
            return key;
        }

        public static List<TV> Values<TK, TV>(this Dictionary<TK, TV> dic){
            List<TV> values = new List<TV>();
            foreach (var data in dic){
                values.Add(data.Value);
            }

            return values;
        }

        public static List<TV> Values<TK, TV>(this Dictionary<TK, TV> dic, List<TV> valuesList){
            valuesList = valuesList ?? new List<TV>();
            valuesList.Clear();
            foreach (var data in dic){
                valuesList.Add(data.Value);
            }

            return valuesList;
        }

        public static List<TK> Keys<TK, TV>(this Dictionary<TK, TV> dic){
            if (dic.Count <= 0){
                return default;
            }

            List<TK> result = new List<TK>();
            foreach (var temp in dic){
                result.Add(temp.Key);
            }

            return result;
        }

        public static List<TK> Keys<TK, TV>(this Dictionary<TK, TV> dic, List<TK> keyList){
            if (dic.Count <= 0){
                return default;
            }

            keyList = keyList ?? new List<TK>();
            keyList.Clear();
            foreach (var temp in dic){
                keyList.Add(temp.Key);
            }

            return keyList;
        }

        public static KeyValuePair<TK, TV> GetByIndex<TK, TV>(this Dictionary<TK, TV> dic, int index){
            if (null == dic){
                Logs.LogError($"[DictionaryExtensions] Dictionary is empty]");
                return default;
            }

            if (index < 0 || index >= dic.Count){
                Logs.LogError($"[DictionaryExtensions] Dictionary is out of range");
                return default;
            }

            int                  current = 0;
            KeyValuePair<TK, TV> rel     = default;
            foreach (var single in dic){
                rel = single;
                current++;
                if (current == index){
                    break;
                }
            }

            return rel;
        }

        public static TK GetKeyByIndex<TK, TV>(this   Dictionary<TK, TV> dic, int index){ return dic.GetByIndex(index).Key; }
        public static TV GetValueByIndex<TK, TV>(this Dictionary<TK, TV> dic, int index){ return dic.GetByIndex(index).Value; }

        public static TV LastValue<TK, TV>(this Dictionary<TK, TV> dic){
            int count = dic.Count;
            return dic.GetValueByIndex(count - 1);
        }

        public static TV FirstValue<TK, TV>(this Dictionary<TK, TV> dic){ return dic.GetValueByIndex(0); }

        public static TK LastKey<TK, TV>(this Dictionary<TK, TV> dic){
            int count = dic.Count;
            return dic.GetKeyByIndex(count - 1);
        }

        public static TK FirstKey<TK, TV>(this Dictionary<TK, TV> dic){ return dic.GetKeyByIndex(0); }

        public static void ReAdd<TK, TV>(this Dictionary<TK, TV> dic, TK key, TV value){
            if (!dic.TryAdd(key, value)){
                dic.Remove(key);
                dic.Add(key, value);
            }
        }

        public static void Ergodic<TK, TV>(this Dictionary<TK, TV> dic, Action<TK, TV> onCheck)
        {
            foreach (var pair in dic)
            {
                onCheck?.Invoke(pair.Key, pair.Value);
            }
        }

        public static void CopyTo<TK, TV>(this Dictionary<TK, TV> origin, Dictionary<TK, TV> target)
        {
            if (null == origin)
            {
                throw new NullReferenceException($"[DictionaryExtensions] Function <CopyTo> Execute error, origin dict can not be null !!!");
            }

            if (null == target)
            {
                target = new();
            }

            origin.Ergodic(
                (k, v) =>
                {
                    if (!target.TryAdd(k, v))
                    {
                        target[k] = v;
                    }
                }
            );
        }

        public static void CopyFrom<TK, TV>(this Dictionary<TK, TV> origin, Dictionary<TK, TV> target)
        {
            if (null == target)
            {
                throw new NullReferenceException($"[DictionaryExtensions] Function <CopyFrom> Execute error, target dict can not be null !!!");
            }

            //哈哈哈 测试看下(Dictionary<TK,TV>)null 这种对象能否进Extension function
            if (null == origin)
            {
                origin = new();
            }

            target.Ergodic(
                (k, v) =>
                {
                    if (!origin.TryAdd(k, v))
                    {
                        origin[k] = v;
                    }
                }
            );
        }
    }
}