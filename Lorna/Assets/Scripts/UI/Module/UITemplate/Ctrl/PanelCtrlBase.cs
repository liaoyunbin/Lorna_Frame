namespace FrameWork_UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// 当前一个面板控制器只控制一个prefab。控制对应的prefab的显隐等
    /// 控制prefab放在哪个canvas下以及对应的层级等。控制UI面板的显示等
    /// prefab加载完成后进行对应的逻辑控制，消息注册监听等
    /// </summary>
    public interface IPanelCtrlBase
    {
        void UIMgrShow(UIMgr _parentMgr, params object[] args);      // 显示时调用：逻辑正常生效，预设体正常显示

        void UIMgrHide();     // 逻辑不再生效，预设体隐藏

        void UIMgrPause();    // 逻辑不再生效，预设体处于显示状态
    }

    public abstract class PanelCtrlBase : IPanelCtrlBase
    {
        //public abstract string UIPackName { get; }
        //public abstract string UIResName { get; }

        /// <summary>
        /// UI 预设体路径
        /// </summary>
        protected abstract string UIResPath { get; }
        /// <summary>
        /// UI层级
        /// </summary>
        public abstract UILayer layer { get; }

        private string m_Name;
        public string Name
        {
            get
            {
                if (m_Name == string.Empty)
                {
                    m_Name = GetType().Name;
                }
                return m_Name;
            }
        }

        /// <summary>
        /// 仅供UIMgr调用
        /// </summary>
        /// <param name="_delete"></param>
        /// <param name="args"></param>
        public virtual void UIMgrShow(UIMgr _parentMgr, params object[] args) { }


        /// <summary>
        /// 仅供UIMgr调用
        /// </summary>
        public virtual void UIMgrHide() { }

        /// <summary>
        /// 仅供UIMgr调用
        /// </summary>
        public virtual void UIMgrPause() { }

    }
    ////备注:是否需要关注特效层级的操作
    ////Ui动画和对应过渡效果
    ////UIController的时候可以进行对应事件注册、按键绑定等一系列操作等
    ////事件分发管理器做一个
}