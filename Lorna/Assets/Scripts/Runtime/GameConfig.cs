using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EscapeGame.Utils{
    /// <summary>
    /// 全局静态通用配置
    /// (包内一些内容设置项)
    /// 这个目前只存Resources根目录,加了资源分包再考虑streaming
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Core")]
    public class GameConfig : ScriptableObject{
    #region Environment

        [Header("目标帧率"), ReadOnly]
        public int FPS = 60;

        [Header("开启鼠标"), ReadOnly]
        public bool EnableCursor = false;

        [Header("当前输入设备丢失时弹出提示")]
        public bool LostControllerTips = true;

    #endregion

        /// <summary>
        /// 游戏Exe名
        /// </summary>
        public string AppShortName;

        /// <summary>
        /// 当前版本号(等同于Application.version) //PlayerSettings.bundleVersion
        /// </summary>
        public string AppVer;

        /// <summary>
        /// 当前资源版本号
        /// </summary>
        public string ResVer = "1";

        [Header("最新存档的版本。默认+1。此值变更时初次进游戏默认删除旧档")]
        /// <summary>
        /// 最新存档的版本。存档新增数据时此值+1
        /// 转自GameStartMgr,当前属于全局通用配置
        /// </summary>
        public int NowStorageVersion = 0;


        [Header("资源包名"), ReadOnly]
        public string ResPack;

        [Header("Shader变体存储路径")]
        public string ShaderVariantPath;

        [Header("资源模式")]
        public EPlayMode PlayMode;

    #region Instance

        public const   string     SAVE_PATH = "GameConfig.asset";
        private static GameConfig _instance;

        public static GameConfig Instance{
            get{
//                if (_instance == null){
//#if UNITY_EDITOR
//                    string createPath = $"{LaunchConst.ROOT_ASSET_PATH}/{SAVE_PATH}";
//                    var    director   = Path.GetDirectoryName(createPath);
//                    if (!Directory.Exists(director)){
//                        Directory.CreateDirectory(director);
//                    }

//                    if (!File.Exists(createPath)){
//                        AssetDatabase.CreateAsset(CreateInstance<GameConfig>(), createPath);
//                    }
//#endif
//                    string loadPath = StringUtils.CutSuffix(SAVE_PATH);
//                    _instance = Resources.Load<GameConfig>(loadPath);
//                }

                return _instance;
            }
        }

    #endregion
    }

    public enum EPlayMode
    {
    }
}