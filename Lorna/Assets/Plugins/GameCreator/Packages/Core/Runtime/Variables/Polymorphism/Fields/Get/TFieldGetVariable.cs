using System;
using System.Text;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TFieldGetVariable
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected IdString m_TypeID = ValueNull.TYPE_ID;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public T Get<T>(Args args)
        {
            object value = this.Get(args);

            if(value == null) {
                //备注：上线后记得把这个日志关了
				//备注: 此问题因为策划获取Property后处理逻辑实际不与Target产生直接关联也不好从根源处处理此问题，所以获取日志等级改位Warning
                UnityEngine.Debug.LogWarning($" arg self is:{args.Self}, arg Target is:{args.Target} 获取失败.\n self 路径为{GetAllPath(args.Self)}\nTarget 路径为{GetAllPath(args.Target)}");
                return default;
            }

            return value switch
            {
                T valueTyped => valueTyped,
                _ => Convert.ChangeType(value, typeof(T)) is T valueConverted
                    ? valueConverted
                    : default
            };
        }

        private StringBuilder st = new StringBuilder();
        private string GetAllPath(GameObject obj, string pix = "_")
        {
            return (obj == null) ? string.Empty : GetAllPath(obj.transform, pix);
        }
        private string GetAllPath(Transform obj, string pix = "_")
        {
            st.Clear();
            while (obj != null)
            {
                st.Insert(0,obj.name);
                st.Insert(0, pix);
                obj = obj.transform.parent;
            }
            return st.ToString();
        }
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract object Get(Args args);
        public abstract override string ToString();
    }
}
