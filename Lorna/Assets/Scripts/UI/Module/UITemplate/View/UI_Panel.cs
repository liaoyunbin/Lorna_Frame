namespace FrameWork_UI
{
    using UnityEngine;

    //转为UTF8格式：
    public partial class UI_Panel : UI_Base
    {
        public Animation wndAnimation;
        [Header("显示动画延迟时间，在时间结束之后会调用showWnd函数的延迟回调")]
        public string showAniName = "show";
        [Header("隐藏动画延迟时间，在时间结束之后会调用hideWnd函数的延迟回调")]
        public string hideAniName = "hide";
        public float showAniTime;
        public float hideAniTime;

    }
}


