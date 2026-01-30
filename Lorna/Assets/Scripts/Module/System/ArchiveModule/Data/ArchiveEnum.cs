
namespace LornaGame.ModuleExtensions.ArchiveModule
{
    /// <summary>
    /// 存档插槽
    /// </summary>
    public enum ArchiveLocation
    {
        /// <summary>
        /// 第一个存档的位置
        /// </summary>
        First = 1,
        /// <summary>
        /// 第二个存档的位置
        /// </summary>
        Second = 2,
        /// <summary>
        /// 第三个存档的位置
        /// </summary>
        Third = 3,
        /// <summary>
        /// 第四个存档的位置
        /// </summary>
        Four = 4,

        BossChallenge = 5,
        /// <summary>
        /// 不得修改此数值，且其余枚举不得超过此值
        /// </summary>
        Max = 10,
    }

    /// <summary>
    /// 插槽下存档路径的枚举
    /// </summary>
    public enum SlotArchivePathEnum
    {
        /// <summary>
        /// 永久存档路径
        /// </summary>
        PermanentPath,
        /// <summary>
        /// 临时存档路径
        /// </summary>
        TemporaryPath,
        /// <summary>
        /// 全局路径
        /// </summary>
        GlobalPath,
        /// <summary>
        /// 当前路径
        /// </summary>
        NowPath,
    }
}

