using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace LornaGame.ModuleExtensions
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        // 2. 如何从 JSON 数据中读取并重构出 Vector2 对象？（反序列化）
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // 将 JSON 数据加载到一个临时的 JObject 中
            JObject jo = JObject.Load(reader);

            // 从 JObject 中取出 x, y, z 的值，并创建一个新的 Vector2
            return new Vector2((float)jo["x"], (float)jo["y"]);
        }

        // 3. 如何将 Vector2 对象写成我们想要的 JSON 格式？（序列化）
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            // 开始写入一个 JSON 对象
            writer.WriteStartObject();

            // 写入 x 属性及其值
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);

            // 写入 y 属性及其值
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);

            // 结束这个 JSON 对象
            writer.WriteEndObject();
        }
    }
}