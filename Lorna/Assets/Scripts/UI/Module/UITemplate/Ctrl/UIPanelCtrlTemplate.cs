namespace FrameWork_UI
{
    using UnityEngine.UI;
    using System.Collections;
    //转为UTF8格式：
    ///
    public partial class UIPanelCtrlTemplate : UIPanelCtrl<UI_Panel>
    {
        protected override string UIResPath { get { return ""; } }
        public override UILayer layer { get; }

        protected override void _onWndInitDone()
        {
        }
        protected override void _onShowWnd()
        {
        }
        protected override void _onHideWnd()
        {
        }
    }
}
