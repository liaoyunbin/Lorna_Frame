using LornaGame.ModuleExtensions;
using System;
using System.Collections.Generic;

namespace LornaGame.ModuleExtensions
{
    /// <summary>
    /// 流程状态机
    /// </summary>
    public partial class ProcedureFSM
    {
        #region private param
        /// <summary>
        /// key:流程类名 value:对应的流程
        /// </summary>
        private Dictionary<string, ProcedureBase> m_ProcedureMap = new Dictionary<string,ProcedureBase>();
        /// <summary>
        /// 当前的流程状态
        /// </summary>
        private ProcedureBase m_CurrentState;

        #endregion

        #region public function
        public ProcedureFSM()
        {
            ResetTransition();
            InitProcedureFSM();
        }
        public void SetDefaultState<T>()
        {
            ChangeState(typeof(T).Name,null);
        }
        public void ChangeState(string _nextState,object _param)
        {
            m_CurrentState?.OnExit();
            m_CurrentState = GetState(_nextState);
            m_CurrentState?.OnEnter(_param);
        }

        public string GetNowTransitionName()
        {
            return m_CurrentState != null ? m_CurrentState.ProcedureTypeName : string.Empty;
        }
        #endregion

        #region private function
        private void InitProcedureFSM()
        {
            var subTypes = ADFUtils.GetAllSubClass(typeof(ProcedureBase), false);
            m_ProcedureMap.Clear();
            foreach (var item in subTypes)
            {
                var data = Activator.CreateInstance(item) as ProcedureBase;
                data.OnInit(item.Name, this);
                m_ProcedureMap.Add(item.Name, data);
            }
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        private ProcedureBase GetState(string _name)
        {
            bool flag = m_ProcedureMap.TryGetValue(_name, out var state);
            return flag ? state : default(ProcedureBase);
        }
        #endregion
    }

}
