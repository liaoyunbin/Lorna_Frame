using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
namespace LornaGame.ModuleExtensions
{
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        // 2. 定义如何将Quaternion对象写入JSON（序列化）
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            // 开始写入一个JSON对象
            writer.WriteStartObject();

            // 依次写入x, y, z, w四个分量
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);

            writer.WritePropertyName("y");
            writer.WriteValue(value.y);

            writer.WritePropertyName("z");
            writer.WriteValue(value.z);

            writer.WritePropertyName("w");
            writer.WriteValue(value.w);

            // 结束对象写入
            writer.WriteEndObject();
        }

        // 3. 定义如何从JSON中读取并重建Quaternion对象（反序列化）
        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            // 将JSON数据加载到一个临时容器中
            JObject jo = JObject.Load(reader);

            // 从容器中取出x, y, z, w的值，并创建一个新的Quaternion
            return new Quaternion(
                (float)jo["x"],
                (float)jo["y"],
                (float)jo["z"],
                (float)jo["w"]
            );
        }
    }
}