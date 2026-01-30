
using FrameWork_Tools;
using System;
using UnityEngine;

namespace FrameWork_UI
{
    //        /* 开启事件顺序 _onWndInitDone -> _onShowWnd -> _onAfterOpen
    //            _onWndInitDone  ui资源加载完成调用
    //            _onShowWnd active = true 时调用
    //            _onAfterOpen 开启动画播放完成后调用
    //         */


    //逻辑：UImgr进行show的时候
    //获取对应prefab，设置canvas以及位置等一系列数据。并获得对应mono. 执行UI加载完的逻辑
    //而后执行对应的动画表现，等动画表现完后执行_onshowWnd操作， 并将对应的参数传进去做处理

    //关闭的时候，有mono的话直接执行对应关闭逻辑，否则关闭不生效
    //恢复逻辑，打开对应面板，而后重新show一次
    public abstract class UIPanelCtrl<T> : PanelCtrlBase where T : UI_Panel
    {
        /** 脚本对象*/
        private T m_MonoWnd;
        public T wnd { get { return m_MonoWnd; } private set { m_MonoWnd = value; } }

        /// <summary>
        /// 加载数量
        /// </summary>
        private int m_LoadedCount = 0;


        /// <summary>
        /// 仅供UIMgr调用
        /// </summary>
        public override void UIMgrShow(UIMgr _parentMgr, params object[] args)
        {
            m_LoadedCount++;
            if (wnd != null)
            {
                //设置窗口有效
                getGameObj().SetActive(true);
                //调用事件函数
                _onShowWnd();
                //处理动画，然后延迟处理函数
                _dealAniAction(wnd.showAniName, wnd.showAniTime, null, null);
                return;
            }

            GameObject parent = _parentMgr.GetLayerParent(layer);
            bool success = LoadUI(parent == null?null:parent.transform);
            if(!success)
            {
                return;
            }
            _dealAniAction(wnd.showAniName, wnd.showAniTime, () => DoShow(args), null);
        }
        /// <summary>
        /// 仅供UIMgr调用
        /// </summary>
        public override void UIMgrHide() {
            m_LoadedCount--;
            _onHideWnd();
            if (wnd != null)
            {
                _dealAniAction(wnd.hideAniName, wnd.hideAniTime, null, DoHide);
                return;
            }
        }

        /// <summary>
        /// 仅供UIMgr调用
        /// </summary>
        public override void UIMgrPause() { }
        #region private function
        /// <summary>
        /// 获取用于操作的窗口对象
        /// </summary>
        /// <returns></returns>
        private GameObject getGameObj()
        {
            if (null == wnd)
            {
                return null;

            }
            return wnd.gameObject;
        }

        private bool LoadUI(Transform _parent)
        {
            var newGo = WrapMgr.assetManager.LoadAsset<GameObject>(UIResPath);
            if (null == newGo)
            {
                Debug.LogError(UIResPath + " 找不到对应的GameObject");
                return false;
            }
            newGo.transform.SetParent(_parent, false);
            newGo.transform.localPosition = Vector3.zero;
            //重置尺寸
            newGo.transform.localScale = new Vector3(1, 1, 1);
            newGo.SetActive(false);
            wnd = newGo.GetComponent<T>();
            if (null == wnd)
            {
                Debug.LogError(UIResPath + " 上的GameObject对应的mono脚本");
                return false;
            }
            //设置窗口初始状态隐藏
            getGameObj().SetActive(false);
            //窗口初始化完成
            _onWndInitDone();
            //设置UI初始化完成
            return true;
        }

        private void DoShow(params object[] args)
        {
            getGameObj().SetActive(true);
            _onShowWnd();
        }
        private void DoHide()
        {
            getGameObj().SetActive(false);
        }
        /// <summary>
        /// 在播放对应动作后执行指定函数
        /// </summary>
        private void _dealAniAction(string _aniName, float _delayTime, Action _doneAction, Action _aniDoneAction)
        {
            //有动画时，等动画播放完后再处理函数
            if (null != wnd && null != wnd.wndAnimation)
            {
                AnimationClip clip = wnd.wndAnimation.GetClip(_aniName);
                if (null != clip)
                {
                    float time = _delayTime;
                    if (time < 0.001f)
                        time = clip.length;
                    wnd.wndAnimation.Play(_aniName, PlayMode.StopAll);

                    //todo:延迟处理事件
                    int millisecondsDelay = (int)(time * 1000);
                    WrapMgr.taskManager.DoTask(_doneAction, millisecondsDelay);
                    WrapMgr.taskManager.DoTask(_aniDoneAction, millisecondsDelay);
                    return;
                }
            }
            //调用事件函数
            if (null != _doneAction)
                _doneAction();
            if (null != _aniDoneAction)
                _aniDoneAction();
        }

        #endregion

        #region abstract

        /// <summary>
        /// 窗口初始化完成调用的函数
        /// </summary>
        protected abstract void _onWndInitDone();

        /// <summary>
        /// 显示窗口的事件函数
        /// </summary>
        protected abstract void _onShowWnd();

        /// <summary>
        /// 隐藏窗口的事件函数
        /// </summary>
        protected abstract void _onHideWnd();
        #endregion
    }
}