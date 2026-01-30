using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace LornaGame.ModuleExtensions.ArchiveModule
{
    /// <summary>
    /// 受存档位置影响，受临时存档和永久存档影响
    /// </summary>
    [Serializable]
    public class SlotSaveData
    {
        // TODO: 
        // 要存啥，就手动实现一个对应IComponent类型。
        // 存储方式 - 自动将特定IComponent类型中 "公共字段" 或 "以[JsonProperty]属性" 修饰的字段，写入Json数据中。
        // 三种数据 - 临时存档数据、大存档数据、不常发生改变的设置数据或静态数据。

        [JsonProperty]
        private Dictionary<Type, ISlotSaveComponent> allComponnet = new Dictionary<Type, ISlotSaveComponent>();

        public SlotSaveData()
        {
            var subTypes = ADFUtils.GetImplementingTypes(typeof(ISlotSaveComponent), false);
            allComponnet.Clear();
            foreach (var item in subTypes)
            {
                allComponnet.Add(item, Activator.CreateInstance(item) as ISlotSaveComponent);
            }
        }

        public T GetComponent<T>() where T : ISlotSaveComponent
        {
            bool flag = allComponnet.TryGetValue(typeof(T), out var component);
            return flag ? (T)component : default(T);
        }
    }
}