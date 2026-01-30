
using Newtonsoft.Json;
using System.Collections.Generic;
namespace LornaGame.ModuleExtensions.ArchiveModule
{
    public class SlotArchiveData
    {
        /// <summary>
        ///当前数据。受临时存档和永久存档影响
        /// </summary>
        public SlotSaveData saveData = new SlotSaveData();
        /// <summary>
        ///当前数据。不受临时存档和永久存档影响
        /// </summary>
        public SlotGlobalData globalData = new SlotGlobalData();


        public T GetSaveComponent<T>() where T : ISlotSaveComponent
        {
            return saveData.GetComponent<T>();
        }
        public T GetGlobalComponent<T>() where T : ISlotGlobalComponent
        {
            return globalData.GetComponent<T>();
        }

    }
}
