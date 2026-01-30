using LornaGame.ModuleExtensions;
using System;
using System.Collections.Generic;
namespace LornaGame.Runtime
{
    /// <summary>
    /// buff效果管理器
    /// 1、层数：       没超过最大层数，进行层数叠加，不得超出上限。超出上限时进行报错提示。叠层时如果有持续时间，持续时间重置
    /// 2、移除方式：   目前buff移除两种情况，一种持续时间结束；一种外界进行移除。 
    /// 3、buff类型：   根据不同的buff类型作用不同的buff效果
    /// 4、备注：       buff间执行先后差异不应造成效果不同
    /// </summary>
    public class BuffEffctCtrl
    {
        /// <summary>
        /// 当前所有的buff数据     key：buffID
        /// </summary>
        private Dictionary<string, BuffEffectBase> m_BuffMap = new Dictionary<string, BuffEffectBase>();
        private List<BuffEffectBase> m_NeedRemoveBuffList = new List<BuffEffectBase>();


        public void UnInit()
        {
            DeleteAllBuff();
        }

#if UNITY_EDITOR
        public Dictionary<string, BuffEffectBase> GetAllBuff()
        {
            return m_BuffMap; 
        }
#endif

        private void RefreshImmediately()
        {
            OnBuffUpdate(0);
        }
        /// <summary>
        /// 实时更新buff.buff结束的话，进行卸载
        /// </summary>
        /// <param name="_deltaTime"></param>
        public void OnBuffUpdate(float _deltaTime)
        {
            m_NeedRemoveBuffList.Clear();

            foreach (var i in m_BuffMap)
            {
                try
                {
                    if (i.Value != null)
                    {
                        i.Value.Update(_deltaTime);
                        if (i.Value.IsEnd)
                        {
                            m_NeedRemoveBuffList.Add(i.Value);
                            i.Value.UnInit();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logs.LogError("处理buff更新时出错， buffId为{0}。 报错提示为{1}", i.Key, e);
                }
            }
            int count = m_NeedRemoveBuffList.Count;
            for (int i = 0; i < count; i++)
            {
                BuffEffectBase temp = m_NeedRemoveBuffList[i];
                if (m_BuffMap.Remove(temp.Id))
                {
                    BuffPoolMgr.Instance.Recycle(temp);
                }
            }
        }

        /// <summary>
        /// 删除所有buff,完成
        /// </summary>
        public void DeleteAllBuff()
        {
            foreach (var i in m_BuffMap)
            {
                i.Value.IsEnd = true;
            }
            RefreshImmediately();
        }

        /// <summary>
        /// 是否有此buff
        /// </summary>
        /// <param name="_bufferName"></param>
        /// <returns></returns>
        public bool HasBuff(string _bufferName)
        {
            return m_BuffMap.ContainsKey(_bufferName);
        }

        /// <summary>
        /// 移除buff层数。层数<=0时buff移除
        /// </summary>
        public void RemoveBufferLayer(string _bufferName, int _count = 1)
        {
            //Logs.Log("RemoveBufferLayer{0}", _bufferName);
            if (m_BuffMap.ContainsKey(_bufferName))
            {
                m_BuffMap[_bufferName].ReduceLayer(_count);
            }
            RefreshImmediately();
        }

       
        public void AddBuffer(string _bufferName, BuffEffectType _buffEffectType, int _count = 1)
        {
            //Logs.Log("Addbuffer{0}", _bufferName);
            if (m_BuffMap.ContainsKey(_bufferName))
            {
                m_BuffMap[_bufferName].AddLayerCount(_count);
            }
            else
            {
                //初始化完之后再加入字典
                BuffEffectBase temp = BuffPoolMgr.Instance.CreateBuff(_buffEffectType);
                temp.Init(_bufferName, _count);
                m_BuffMap.Add(_bufferName, temp);
            }
            RefreshImmediately();
        }
    }
}