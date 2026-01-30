namespace LornaGame.UIExtensions
{
    using UnityEngine.UI;
    using System.Collections;
    //转为UTF8格式：
    ///
    public partial class PopupC1_PanelCtrl : UIPanelCtrl<PopupC1_Design>
    {
        // 本脚本只在不存在时会生成一次。已存在不会再次生成覆盖
        protected override string UIResPath { get { return "UI/Panel/PopupC1"; } }
        public override UILayer layer { get { return UILayer.Popup; } }

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
