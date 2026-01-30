namespace FrameWork_UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using FrameWork_Tools;

    public static class Layer
    {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int IgnoreRaycast = 2;
        public const int Water = 4;
        public const int UI = 5;
    }
    //转为UTF8格式：
    public partial class UIMgr : Singleton<UIMgr>
    {
        // 层级栈
        private Dictionary<UILayer, Stack<PanelCtrlBase>> uiStacks = new Dictionary<UILayer, Stack<PanelCtrlBase>>();

        public UIMgr()
        {
            m_InitCanvas = false;
            EventMgr.Instance.Subscribe(UIConstDefaults.EventOnUpdate, DoUpdate);
            InitControllers();
            InitStacks();
        }
        #region private function
        private void InitStacks()
        {
            // 初始化层级栈
            uiStacks.Clear();
            var list = (UILayer[])Enum.GetValues(typeof(UILayer));
            foreach (var item in list)
            {
                uiStacks.Add(item, new Stack<PanelCtrlBase>());
            }
        }

        #endregion

        #region public function

        /// <summary>
        /// 关闭所有UI
        /// </summary>
        public void CloseAllUI()
        {
            // 按弹窗→正常→背景顺序关闭
            LayerClearAndPopTopUI(UILayer.Popup);
            LayerClearAndPopTopUI(UILayer.Normal);
            LayerClearAndPopTopUI(UILayer.Background);
        }

        /// <summary>
        /// 打开UI（统一接口）
        /// 自动处理栈逻辑
        /// </summary>
        public void OpenUI<T>(params object[] args) where T : PanelCtrlBase
        {
            T enterUI = GetController<T>();
            UILayer enterLayer = enterUI.layer;
            Stack<PanelCtrlBase> stack = uiStacks[enterLayer];

            switch (enterLayer)
            {
                case UILayer.Background: // A层：替换逻辑
                    LayerClearAndPopTopUI(UILayer.Popup);
                    LayerClearAndPopTopUI(UILayer.Normal);
                    LayerPopTopUI(UILayer.Background);

                    // 新背景入栈并显示
                    stack.Push(enterUI);
                    enterUI.UIMgrShow(this, args);
                    break;

                case UILayer.Normal: // B层：隐藏上一个，显示当前

                    //清空popUp层
                    LayerClearAndPopTopUI(UILayer.Popup);

                    //之前没有normal界面打开，底部有背景层，禁用背景层逻辑，新UI入栈并显示
                    if (stack.Count <= 0)
                    {
                        Stack<PanelCtrlBase> bgStack = uiStacks[UILayer.Background];
                        if (bgStack.Count > 0)
                        {
                            bgStack.Peek().UIMgrPause();
                        }
                        // 新UI入栈并显示
                        stack.Push(enterUI);
                        enterUI.UIMgrShow(this, args);
                        break;
                    }

                    //之前有normal界面打开，栈顶是自己，重新刷新
                    if (stack.Peek() == enterUI)
                    {
                        enterUI.UIMgrShow(this, args);
                        break;
                    }

                    //栈中原先有此UI。之前有normal界面打开，栈顶不是自己，原先的Normal栈里包含了此数据
                    if (stack.Contains(enterUI))
                    {
                        //隐藏当前正常层栈顶
                        if (stack.Count > 0)
                        {
                            PanelCtrlBase currentNormal = stack.Pop();  // 查看栈顶（不弹出）
                            currentNormal.UIMgrHide();
                        }
                        while (stack.Peek() != enterUI)
                        {
                            stack.Pop();
                        }
                        stack.Peek().UIMgrShow(this, args);
                    }
                    else
                    {
                        //栈顶不是自己， 隐藏当前正常层栈顶
                        if (stack.Count > 0)
                        {
                            PanelCtrlBase currentNormal = stack.Peek();  // 查看栈顶（不弹出）
                            currentNormal.UIMgrHide();
                        }
                        // 新UI入栈并显示
                        stack.Push(enterUI);
                        enterUI.UIMgrShow(this, args);
                    }
                    break;

                case UILayer.Popup: // C层：替换逻辑

                    if (stack.Count <= 0)
                    {
                        Stack<PanelCtrlBase> bgStack = uiStacks[UILayer.Normal];
                        if (bgStack.Count > 0)
                        {
                            bgStack.Peek().UIMgrPause();
                        }
                        // 新UI入栈并显示
                        stack.Push(enterUI);
                        enterUI.UIMgrShow(this, args);
                        break;
                    }

                    // 隐藏当前弹窗
                    PanelCtrlBase currentPopup = stack.Pop();
                    currentPopup.UIMgrHide();
                    // 新弹窗入栈并显示
                    stack.Push(enterUI);
                    enterUI.UIMgrShow(this, args);
                    break;
            }

            Debug.Log($"[OpenUI] {enterUI.Name} | 层级: {enterLayer} | 栈大小: {stack.Count}");
        }
        /// <summary>
        /// 层级界面清空并关闭当前层的顶部UI
        /// </summary>
        /// <param name="layer"></param>
        private void LayerClearAndPopTopUI(UILayer layer)
        {
            Stack<PanelCtrlBase> stack = uiStacks[layer];
            if (stack.Count > 0)
            {
                PanelCtrlBase ui = stack.Pop();
                ui.UIMgrHide();
            }
            //清空当前层栈数据
            stack.Clear();
        }

        /// <summary>
        /// 关闭当前层的顶部UI
        /// </summary>
        private bool LayerPopTopUI(UILayer layer)
        {
            Stack<PanelCtrlBase> stack = uiStacks[layer];
            if (stack.Count > 0)
            {
                PanelCtrlBase ui = stack.Pop();
                ui.UIMgrHide();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 显示当前层栈顶UI
        /// </summary>
        /// <param name="layer"></param>
        private bool ShowTopUI(UILayer layer)
        {
            Stack<PanelCtrlBase> stack = uiStacks[layer];
            // 显示上一个UI（如果有）
            if (stack.Count > 0)
            {
                PanelCtrlBase previousUI = stack.Peek();
                previousUI.UIMgrShow(this);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 关闭当前显示的UI（统一接口）
        /// 按照 C层→B层→A层 优先级关闭
        /// </summary>
        public void CloseNowUI()
        {
            //如果有弹窗，优先关闭弹窗，打开上一层； 没有弹窗，优先关闭normal，打开上一层
            if (LayerPopTopUI(UILayer.Popup) || LayerPopTopUI(UILayer.Normal))
            {
                bool normalCanOpen = ShowTopUI(UILayer.Normal);
                if (!normalCanOpen)
                {
                    ShowTopUI(UILayer.Background);
                }
                return;
            }
            LayerPopTopUI(UILayer.Background);

            Debug.LogWarning("[CloseNowUI] 没有可关闭的UI");
        }
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// 获取栈信息
        /// </summary>
        public string GetStackInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== UI栈状态 ===");

            foreach (var kvp in uiStacks)
            {
                sb.AppendLine($"{kvp.Key}层: {kvp.Value.Count} 个");

                if (kvp.Value.Count > 0)
                {
                    sb.AppendLine($"  当前显示: {kvp.Value.Peek().Name}");

                    // 显示栈中所有UI
                    var stackArray = kvp.Value.ToArray();
                    for (int i = stackArray.Length - 1; i >= 0; i--)
                    {
                        sb.AppendLine($"  {stackArray.Length - i}. {stackArray[i].Name}");
                    }
                }
            }

            return sb.ToString();
        }
#endif
    }
}

