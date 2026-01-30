using System.Collections.Generic;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LornaGame.ModuleExtensions{
    public static class JsonUtils{
        public static JsonSerializerSettings S_FullSetting =  new JsonSerializerSettings{ ReferenceLoopHandling = ReferenceLoopHandling.Ignore,Formatting = Formatting.Indented,TypeNameHandling = TypeNameHandling.All };

        /// <summary>
        /// json字符串转对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonContent">json字符串</param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonContent){
            T obj = default;
            obj = JsonConvert.DeserializeObject<T>(jsonContent);
            if (obj != null){
                return obj;
            }

            return default(T);
        }

        /// <summary>
        /// 对象转json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string ObjectToJson<T>(T obj, Formatting formatting = Formatting.None){
            string result = JsonConvert.SerializeObject(obj, formatting);
            return result;
        }

        public static string ObjectToJson<T>(T obj, bool format){
            string result = format ? JsonConvert.SerializeObject(obj, Formatting.Indented) : JsonConvert.SerializeObject(obj);
            return result;
        }

        public static string ObjectToJson<T>(T obj, JsonSerializerSettings setting)
        {
            var json = JsonConvert.SerializeObject(obj, setting);
            return json;
        }
        public static T JsonToObject<T>(string jsonContent, JsonSerializerSettings setting)
        {
            T obj = JsonConvert.DeserializeObject<T>(jsonContent, setting);
            if (obj != null)
            {
                return obj;
            }

            return default(T);
        }
    }
}