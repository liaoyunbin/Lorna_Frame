using HybridCLR.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace EscapeGame.Building{
    [CommandPkgStr(shortCommand : "hy", fullCommand : "Hybrid")]
    public class BuildingCmdHybrid : IBuildingCommand{
        public string                     Name          => "热更";
        public string                     Help          => "构建热更程序集并放到指定文件夹";
        public BuildingCmdValueType       CmdValType    => BuildingCmdValueType.Choices;

        public Dictionary<string, string> Choices { get; } = new() { { "full", "生成全部【出包前】" }, { "patch", "热更【出包后】" } };
        public Action<string>             OnEditorExtra => null;

        private string tagetFolder = string.Concat(Application.dataPath, "/Game/Res/AssetBundleRes/DLL");

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
            //运行菜单 HybridCLR/ Generate / All 完之後
            if (arg.Equals("full"))
            {
                PreGenerate();
                GenerateRequireComponenTool.GenerateRequireComponentData();
                HybridGenAll();
            }
            else if (arg.Equals("patch"))
            {
                GenerateRequireComponenTool.GenerateRequireComponentData();
                DoHotFix();
            }

            // 1. 删除文件夹下所有文件
            DeleteFilesInFolder(tagetFolder);
            CopyAOTAssembliesToTargetFolder();
            CopyHotUpdateAssembliesToTargetFolder();

            //
            //CopyHybridDLL();
            UnityEngine.Debug.Log("lorna CopyHybridDLL");
            return true;
        }


        #region private function

        /// <summary>
        /// 出包前生成人更程序
        /// </summary>
        private void HybridGenAll()
        {
            EditorApplication.ExecuteMenuItem("HybridCLR/Generate/All");
        }

        /// <summary>
        /// 出包后重新编译热更新代码
        /// </summary>
        private void DoHotFix()
        {
            EditorApplication.ExecuteMenuItem("HybridCLR/CompileDll/ActiveBuildTarget");
        }

        private void CopyAOTAssembliesToTargetFolder()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = tagetFolder;

            foreach (var dll in SettingsUtil.AOTAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    UnityEngine.Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                UnityEngine.Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        private void CopyHotUpdateAssembliesToTargetFolder()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir = tagetFolder;
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                UnityEngine.Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }
        private async Task PreGenerate()
        {
            HybridCLR.Editor.Commands.AOTReferenceGeneratorCommand.CompileAndGenerateAOTGenericReference();
            string[] old = HybridCLR.Editor.Settings.HybridCLRSettings.Instance.patchAOTAssemblies;
            IReadOnlyList<string> curList = AOTGenericReferences.PatchedAOTAssemblyList;
            curList = FilterDllName(curList);

            if (old == null || old.Length != curList.Count || !JudgeEquals(curList, old))
            {
                PreWriteContent(curList);
            }
        }

        private static IReadOnlyList<string> FilterDllName(IReadOnlyList<string> curList)
        {
            List<string> rel = new List<string>();
            int count = curList.Count;
            for (int i = 0; i < count; ++i)
            {
                string tempStr = curList[i];
                int startIndex = tempStr.IndexOf(".dll");
                string replace = tempStr.Remove(startIndex, 4);
                rel.Add(replace);
            }

            return rel;
        }

        private bool JudgeEquals(IReadOnlyList<string> curList, string[] old)
        {
            int len = curList.Count;
            for (int i = 0; i < len; ++i)
            {
                string tempName = curList[i];
                if (!old.Contains(tempName))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task PreWriteContent(IReadOnlyList<string> curList)
        {
            int count = curList.Count;
            string[] wr = new string[count];
            for (int i = 0; i < count; i++)
            {
                wr[i] = curList[i];
            }

            HybridCLR.Editor.Settings.HybridCLRSettings.Instance.patchAOTAssemblies = wr;
            HybridCLR.Editor.Settings.HybridCLRSettings.Save();
        }

        /// <summary>
        /// 删除文件夹里的所有文件
        /// </summary>
        /// <param name="folderPath"></param>
        private void DeleteFilesInFolder(string folderPath)
        {
            // 0. 安全检查
            if (!Directory.Exists(folderPath))
            {
                UnityEngine.Debug.LogError($"路径不存在: {folderPath}");
                return;
            }

            try
            {
                // 1. 删除所有文件
                string[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                    UnityEngine.Debug.Log($"已删除: {file}");
                }

                // 2. 删除所有子文件夹（可选）
                string[] subDirs = Directory.GetDirectories(folderPath);
                foreach (string dir in subDirs)
                {
                    Directory.Delete(dir, true); // true表示递归删除子内容
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"删除失败: {e.Message}");
            }
        }
        #endregion
    }
}