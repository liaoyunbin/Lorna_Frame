namespace LornaGame.UIExtensions
{
    using System;
    using System.Collections.Generic;
    using LornaGame.ModuleExtensions;

    //转为UTF8格式：
    public partial class UIMgr
    {
        #region private param
        /// <summary>
        /// 所有的UI控制器
        /// </summary>
        private Dictionary<string, PanelCtrlBase> m_AllControllers = new Dictionary<string, PanelCtrlBase>();
        #endregion

        #region private function
        /// <summary>
        /// 初始化注册所有的控制器
        /// </summary>
        private void InitControllers()
        {
            var subTypes = ADFUtils.GetImplementingTypes(typeof(PanelCtrlBase), false);
            m_AllControllers.Clear();
            foreach (var item in subTypes)
            {
                PanelCtrlBase con = Activator.CreateInstance(item) as PanelCtrlBase;
                m_AllControllers.Add(item.Name, con);
            }

        }
        private T GetController<T>() where T : PanelCtrlBase
        {
            bool flag = m_AllControllers.TryGetValue(typeof(T).Name, out var component);
            return flag ? (T)component : default(T);
        }
        //private UIBaseController GetController(string name)
        //{
        //    m_AllControllers.TryGetValue(name, out var component);
        //    return component;
        //}
        #endregion

    }
}
