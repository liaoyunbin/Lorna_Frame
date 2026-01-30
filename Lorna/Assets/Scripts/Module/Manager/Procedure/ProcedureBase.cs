namespace LornaGame.ModuleExtensions
{

    /// <summary>
    /// 流程状态基类。
    /// </summary>
    public abstract class ProcedureBase
    {
        /// <summary>
        ///  = typeof(Class).Name. 对应流程类型名
        /// </summary>
        public string ProcedureTypeName
        {
            get; private set;
        }

        #region private param
        private ProcedureFSM m_Fsm;
        #endregion

        #region abstract function
        /// <summary>
        /// 有限状态机状态初始化时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public void OnInit(string name, ProcedureFSM _fsm)
        {
            ProcedureTypeName = name;
            m_Fsm = _fsm;
        }

        /// <summary>
        /// 有限状态机状态进入时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public abstract void OnEnter(object _param);

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        public abstract void OnExit();

        #endregion

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        protected void ChangeState<T>(object param = null) where T : ProcedureBase
        {
            var nextState = typeof(T).Name;
            if (!m_Fsm.CanTransition(ProcedureTypeName, nextState))
            {
                Logs.LogError("切换失败，不能从{0}状态切换到{1}状态，清检测 ProcedureFSM 的Transition列表", ProcedureTypeName, nextState);
                return;
            }
            m_Fsm.ChangeState(nextState, param);
        }
    }
}
