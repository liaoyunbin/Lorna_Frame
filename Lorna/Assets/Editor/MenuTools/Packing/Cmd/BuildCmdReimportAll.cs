using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LornaGame.ModuleExtensions;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using LornaGame.ModuleExtensions;

namespace EscapeGame.Building
{
    [CommandPkgStr("r", "reimpot", true)]
    public class BuildCmdReimportAll : IBuildingCommand
    {
        internal enum ReimportAssetsType
        {
            [InspectorName("脚本")]
            Scripts = 0,//脚本
            [InspectorName("单体素材(mat|texture|fbx等)")]
            Single = 1,//mat|texture|fbx等素材
            [InspectorName(" 复合素材(scene|prefab|timeline等)")]
            Composite = 2,//prefab|unity|asset等复合型内容
        }

        #region Const params

        //当前需要Reimport的目录不需要配置，直接写死即可
        private static readonly string[] s_reimportDir = {
            "Game",
            "Plugins",
            "Resources",
            "Settings"
        };

        private static readonly string s_regStr = @"^.+\.(cs|unity|prefab|mat|asset|controller|playable|anim)$";

        private static readonly string s_scriptsRegStr = @"^.+\.(cs)$";

        private static readonly string s_singleRegStr = @"^.+\.(mat|controller|playable|tga|png|fbx)$";

        private static readonly string s_compositeRegStr = @"^.+\.(unity|prefab|asset)$";

        #endregion
        public string Name => "Reimport all!";

        public string Help => $"Reimpot 配置目录所有文件,防止正在打包的机器与提交机之间存在不同的GUID差异性,导致包内表现异常!!! 耗时较长，请耐心等待!!(本应该作为预处理存在的,不过保险起见可以直接作为正式CMD也更保险)";

        public BuildingCmdValueType CmdValType => BuildingCmdValueType.Choices;

        /// <summary>
        /// 当前先只添加ReimportAll上边的目录即可
        /// 后边考虑增加DelPrefabOverrideMissingUnused
        /// </summary>
        public Dictionary<string, string> Choices => new()
        {
            {"reDir","仅ReimportAll项目目录" },
        };

        public Action<string> OnEditorExtra => null;

        private Dictionary<ReimportAssetsType, List<string>> _reimportSortDict = new()
        {
            {ReimportAssetsType.Scripts,new List<string>()},//脚本
            {ReimportAssetsType.Single,new List<string>()},//mat|texture|fbx等素材
            {ReimportAssetsType.Composite,new List<string>()},//prefab|unity|asset等复合型内容
        };


        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output)
        {

            switch (arg)
            {
                case "reDir":
                    ReimportAction();
                    break;
            }

            UnityEngine.Debug.Log($"[BuildCmdReimportAll] ReimportAll完成!".GreenColor());
            return true;
        }

        #region Private functions
        private void ReimportAction()
        {
            int len = s_reimportDir?.Length ?? 0;
            List<string> scripts = new();
            List<string> single = new();
            List<string> composite = new();

            for (int i = 0; i < len; ++i)
            {
                string path = s_reimportDir[i];
                // HookFiles(path, ref scripts);   
                // HookFiles(path, ref single);
                // HookFiles(path, ref composite);
                HookFiles(path, ref scripts, ref single, ref composite);
            }

            _reimportSortDict[ReimportAssetsType.Scripts] = scripts;
            _reimportSortDict[ReimportAssetsType.Single] = single;
            _reimportSortDict[ReimportAssetsType.Composite] = composite;


            len = (int)ReimportAssetsType.Composite;
            for (int i = 0; i <= len; ++i)
            {
                var reimpotType = (ReimportAssetsType)Enum.ToObject(typeof(ReimportAssetsType), i);
                var list = _reimportSortDict[reimpotType];
                string step = $"总进度: {i} / {len} - {GetInspectorName(reimpotType)}";
                ReimportAll(step, list, out double dur, out uint count, (str) => { return true; });
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        #region AssetReimportUtils(Modify from UnityEditor.Rendering.AssetReimportUtils)

        private static void ReimportAll(string step, List<string> files, out double duration, out uint numberOfAssetsReimported, Func<string, bool> importNeedDelegate = null)
        {
            numberOfAssetsReimported = 0;
            duration = 0.0;
            using (TimedScope.FromRef(ref duration))
            {
                try
                {
                    AssetDatabase.StartAssetEditing();
                    int fileLen = files?.Count ?? 0;
                    for (int i = 0; i < fileLen; ++i)
                    {
                        string filePath = files[i];
                        string fileName = GetFileName(filePath);
                        EditorUtility.DisplayProgressBar($"re-import -- {step}", $"({i} of {fileLen}) {fileName}", (float)i / (float)fileLen);
                        if (importNeedDelegate?.Invoke(filePath) ?? true)
                        {
                            AssetDatabase.ImportAsset(filePath);
                            numberOfAssetsReimported++;
                        }
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private static void HookFiles(string dir, ref List<string> files)
        {
            if (null == files)
            {
                files = new();
            }

            string path = Path.Combine(Application.dataPath, dir);

            string[] hookFiles = Directory.GetFiles(path, ".", SearchOption.AllDirectories).Where(it => Regex.IsMatch(it, s_regStr)).ToArray();
            if (hookFiles?.Length > 0)
            {
                files.AddRange(hookFiles);
            }
        }

        private static void HookFiles(string dir, ref List<string> scriptsFiles, ref List<string> singleFiles, ref List<string> compositeFiles)
        {
            if (null == scriptsFiles)
            {
                scriptsFiles = new();
            }
            if (null == singleFiles)
            {
                singleFiles = new();
            }
            if (null == compositeFiles)
            {
                compositeFiles = new();
            }
            string path = Path.Combine(Application.dataPath, dir);
            string[] hookFiles = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
            int len = hookFiles?.Length ?? 0;
            for (int i = 0; i < len; ++i)
            {
                string sPath = hookFiles[i];
                if (Regex.IsMatch(sPath, s_scriptsRegStr))
                {
                    scriptsFiles.Add(sPath);
                }
                else if (Regex.IsMatch(sPath, s_singleRegStr))
                {
                    singleFiles.Add(sPath);
                }
                else if (Regex.IsMatch(sPath, s_compositeRegStr))
                {
                    compositeFiles.Add(sPath);
                }
            }
        }

        private static string GetFileName(string dir)
        {
            int splitSign = dir.LastIndexOf(StringUtils.URL_FORWARD_SIGN);
            if (splitSign < 0)
            {
                splitSign = dir.LastIndexOf(StringUtils.URL_REVERSE_SIGN);
            }

            if (splitSign >= 0)
            {
                return dir.Substring(splitSign + 1);
            }

            return dir;
        }

        private static string GetInspectorName(ReimportAssetsType eType)
        {
            var attribute = eType.GetType().GetMember(eType.ToString())[0].GetCustomAttribute(typeof(InspectorNameAttribute));
            var displayName = ((InspectorNameAttribute)attribute).displayName;
            return displayName;
        }

        #endregion
        #endregion
    }
}