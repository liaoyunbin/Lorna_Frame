namespace FrameWork_UI
{
    using UnityEngine.UI;
    using System.Collections;
    using System.Diagnostics;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using FrameWork_Tools;
    using UnityEditor.Experimental.GraphView;

    //转为UTF8格式：
    ///
    public partial class BackgroundA1_PanelCtrl : UIPanelCtrl<BackgroundA1_Design>
    {
        // 本脚本只在不存在时会生成一次。已存在不会再次生成覆盖
        protected override string UIResPath { get { return "UI/Panel/BackgroundA1"; } }
        public override UILayer layer { get { return UILayer.Background; } }

        protected override void _onWndInitDone()
        {
        }
        protected override void _onShowWnd()
        {
            Init();
            EventMgr.Instance.Subscribe("OnUpdate", DoUpdate);
        }
        protected override void _onHideWnd()
        {
            EventMgr.Instance.Subscribe("OnUpdate", DoUpdate);
        }

        private void Init()
        {
           // wnd.Bg.
            wnd.ButtonConfirm.onClick.RemoveAllListeners();
            wnd.ButtonConfirm_3_.onClick.RemoveAllListeners();
            wnd.ButtonConfirm_1_.onClick.RemoveAllListeners();
            wnd.ButtonConfirm_2_.onClick.RemoveAllListeners();

            wnd.ButtonConfirm.onClick.AddListener(()=>UIMgr.Instance.OpenUI<PopupC2_PanelCtrl>());

            wnd.ButtonConfirm_1_.onClick.AddListener(() => UIMgr.Instance.OpenUI<NormalB2_PanelCtrl>());

            wnd.ButtonConfirm_2_.onClick.AddListener(() => UnityEngine.Debug.Log("按键2点击"));

            wnd.ButtonConfirm_3_.onClick.AddListener(() => UnityEngine.Debug.Log("按键3点击"));

            selectable = wnd.ButtonConfirm_3_;
            EventSystem.current.SetSelectedGameObject(wnd.ButtonConfirm_3_.gameObject);
        }

        private Selectable selectable;
        public float moveDeadZone = 0.6f; // 输入死区，避免误操作
        private AxisEventData m_AxisEventData; // 用于导航的事件数据


        void DoUpdate(System.Object o)
        {
            HandleCustomNavigationInput();
            HandleConfirmationInput();
            UnityEngine.Debug.Log(EventSystem.current.currentSelectedGameObject);
        }

        /// <summary>
        /// 处理自定义导航输入（例如使用IJKL代替方向键）
        /// </summary>
        private void HandleCustomNavigationInput()
        {
            Vector2 movement = Vector2.zero;

            // 示例：使用IJKL键进行导航，你可以替换为任何按键或Input System的输入
            if (Input.GetKeyUp(KeyCode.I)) movement = Vector2.up;      // 上
            else if (Input.GetKeyUp(KeyCode.K)) movement = Vector2.down;  // 下
            else if (Input.GetKeyUp(KeyCode.L)) movement = Vector2.right; // 右
            else if (Input.GetKeyUp(KeyCode.J)) movement = Vector2.left;  // 左

            if (movement != Vector2.zero)
            {
                MoveSelection(movement);
            }
        }
        /// <summary>
        /// 处理确认输入（如空格键或回车键）
        /// </summary>
        private void HandleConfirmationInput()
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
            {
                if (selectable != null)
                {
                    // 获取当前选中的GameObject上的Button组件
                    Button currentButton = selectable.gameObject.GetComponent<Button>();
                    if (currentButton != null)
                    {
                        // 触发该Button的点击事件
                       currentButton.onClick.Invoke();
                        // 或者可以执行其他自定义逻辑，例如：
                        // ExecuteEvents.Execute(currentButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                UnityEngine.Debug.Log("11111");
                UIMgr.Instance.CloseNowUI();
            }
        }
        /// <summary>
        /// 根据输入方向移动当前选中的UI元素
        /// </summary>
        /// <param name="moveVector">移动方向</param>
        void MoveSelection(Vector2 moveVector)
        {
            // 创建或获取AxisEventData
            if (m_AxisEventData == null)
                m_AxisEventData = new AxisEventData(EventSystem.current);

            m_AxisEventData.Reset();
            m_AxisEventData.moveVector = moveVector;
            m_AxisEventData.moveDir = DetermineMoveDirection(moveVector.x, moveVector.y, moveDeadZone);

            Selectable com = null;
            switch (m_AxisEventData.moveDir)
            {
                case MoveDirection.Right:
                    com = selectable.FindSelectableOnRight();
                    break;
                case MoveDirection.Up:
                    com = selectable.FindSelectableOnUp();
                    break;
                case MoveDirection.Left:
                    com = selectable.FindSelectableOnLeft();
                    break;
                case MoveDirection.Down:
                    com = selectable.FindSelectableOnDown();
                    break;
            }
            if (com!= null)
            {
                selectable = com;
                UnityEngine.Debug.Log(selectable.name);
            }
        }

        /// <summary>
        /// 根据输入向量确定移动方向
        /// </summary>
        MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            if (new Vector2(x, y).sqrMagnitude < deadZone * deadZone)
                return MoveDirection.None;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                return x > 0 ? MoveDirection.Right : MoveDirection.Left;
            }
            else
            {
                return y > 0 ? MoveDirection.Up : MoveDirection.Down;
            }
        }
    }
}
