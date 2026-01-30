using LornaGame.ModuleExtensions;
using System.Collections.Generic;
namespace LornaGame.Runtime
{
    public enum BuffEffectType
    {
        None,
    }
    public class BuffPoolMgr: Singleton<BuffPoolMgr>
    {
        private Dictionary<BuffEffectType, Queue<BuffEffectBase>> m_BUffMap = new Dictionary<BuffEffectType, Queue<BuffEffectBase>>();

        public BuffEffectBase CreateBuff(BuffEffectType _type)
        {
            bool flag = m_BUffMap.TryGetValue(_type, out var _queue);
            if (!flag)
            {
                _queue = new Queue<BuffEffectBase>();
                m_BUffMap.Add(_type, _queue);
            }
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            switch (_type)
            {
                case BuffEffectType.None:
                     return new BuffEffect_None();
                default:
                    Logs.LogError("BuffPoolMgr 脚本中 {0}类型buff尚未被实现。", _type);
                    return null;
            }
        }


        public void Recycle<T>(T _buff) where T: BuffEffectBase
        {
            BuffEffectType _type = _buff.BuffType;
            bool flag = m_BUffMap.TryGetValue(_type, out var _queue);
            if (!flag)
            {
                m_BUffMap.Add(_type, new Queue<BuffEffectBase>());
            }
            _queue.Enqueue(_buff);
        }
    }
}