namespace LornaGame.UIExtensions
{

    //转为UTF8格式：

    /// <summary>
    /// 不同的层级，确保UI能正确遮挡
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 背景层，替换关系
        /// </summary>
        Background = 0,
        /// <summary>
        /// 普通层，替换关系
        /// </summary>
        Normal = 100,
        /// <summary>
        /// 弹窗层，替换关系
        /// </summary>
        Popup = 200,

        /// <summary>
        /// 无操作的最顶层，叠加
        /// </summary>
        Top_NO_OPS = 300,
    }
}
