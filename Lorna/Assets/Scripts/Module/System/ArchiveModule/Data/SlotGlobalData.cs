using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace LornaGame.ModuleExtensions.ArchiveModule
{
    /// <summary>cun'da
    /// 受存档位置影响，不受临时存档和永久存档影响
    /// </summary>
    [Serializable]
    public class SlotGlobalData
    {
        // 存储方式 - 自动将特定IComponent类型中 "公共字段" 或 "以[JsonProperty]属性" 修饰的字段，写入Json数据中。
        [JsonProperty]
        private Dictionary<Type, ISlotGlobalComponent> allComponnet = new Dictionary<Type, ISlotGlobalComponent>();

        public SlotGlobalData()
        {
            var subTypes = ADFUtils.GetImplementingTypes(typeof(ISlotGlobalComponent), false);
            allComponnet.Clear();
            foreach (var item in subTypes)
            {
                allComponnet.Add(item, Activator.CreateInstance(item) as ISlotGlobalComponent);
            }
        }

        public T GetComponent<T>() where T : ISlotGlobalComponent
        {
            bool flag = allComponnet.TryGetValue(typeof(T), out var component);
            return flag ? (T)component : default(T);
        }
    }
}