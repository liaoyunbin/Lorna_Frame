using LornaGame.ModuleExtensions;
using System.Collections.Generic;

namespace LornaGame.ModuleExtensions
{
    /// <summary>
    /// 流程切换
    /// </summary>
    public partial class ProcedureFSM
    {
        //判定某个_fromName的流程状态 是否能切换到 _toName的流程状态
        public bool CanTransition(string _fromName, string _toName)
        {
            return m_Transitions.TryGetValue(_fromName, out var transition) && transition.Contains(_toName);
        }

        public void AddTransition<T1, T2>() where T1 : ProcedureBase where T2 : ProcedureBase
        {
            AddTransition(typeof(T1).Name, typeof(T2).Name);
        }
        public void AddTwoWayTransition<T1, T2>() where T1 : ProcedureBase where T2 : ProcedureBase
        {
            AddTransition(typeof(T1).Name, typeof(T2).Name);
            AddTransition(typeof(T2).Name, typeof(T1).Name);
        }
        public Dictionary<string, HashSet<string>> GetAllTransition()
        {
            return m_Transitions;
        }

        #region private param
        /// <summary>
        ///key: ProcedureBase name  value：可以到达的Transition Name
        /// </summary>
        private Dictionary<string, HashSet<string>> m_Transitions = new Dictionary<string, HashSet<string>>();
        #endregion

        #region private function
        private void ResetTransition()
        {
            m_Transitions.Clear();
        }


        private void AddTransition(string _fromName, string _toName)
        {
            if (!m_Transitions.TryGetValue(_fromName, out var data))
            {
                data = new HashSet<string>();
                m_Transitions.Add(_fromName, data);
            }
            if (!data.Contains(_toName))
            {
                data.Add(_toName);
            }
        }
        #endregion
    }
}
