using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EscapeGame.Building.Config;
using EscapeGame.Building.Utils;
using LornaGame.ModuleExtensions;
using EscapeGame.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace EscapeGame.Building{
    //TODO:有些平台setting需要具体出包再做额外设置，比如switch和ps4/5
    [CommandPkgStr("b", "build", true)]
    public class BuildingCmdBuildApp : IBuildingCommand{
        private enum BuildType{
            //构建用的就不给None
            Release          = 0,
            Debug            = 1,
            DebugDevelopment = 2,
        }

        public string               Name       => "构建包体";
        public string               Help       => null;
        public BuildingCmdValueType CmdValType => BuildingCmdValueType.Choices;

        public Dictionary<string, string> Choices{ get; } =
            new(){ { "debug", "构建Debug测试包" },{ "debugDevelopment", "构建Debug自动连接Profiler测试包" },{ "release", "构建发布包" } }; //当前没有移动平台，不涉及证书,有涉及时再来修改

        public  Action<string> OnEditorExtra => null;
        private BuildType      _buildType;

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
            switch (arg){
                case "release":          _buildType = BuildType.Release; break;
                case "debug":            _buildType = BuildType.Debug; break;
                case "debugDevelopment": _buildType = BuildType.DebugDevelopment; break;
            }

            bool               isRelease      = _buildType == BuildType.Release;
            var                assetAppConfig = GameConfig.Instance;
            string             packageName    = assetAppConfig.AppShortName;
            string             buildPath      = string.Empty;
            BuildPlayerOptions buildOption    = new BuildPlayerOptions();
            PlayerSettings.productName = packageName;
            switch (EditorUserBuildSettings.activeBuildTarget){
                case BuildTarget.Android:{
                    //Android的Graphic做的时候再考虑
                    {
                        // PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                    }
                    PlayerSettings.Android.useAPKExpansionFiles = isRelease;
                    EditorUserBuildSettings.buildAppBundle = isRelease;
                    PlayerSettings.Android.keystoreName = "Packing/Android/xxx.keystore"; //TODO:创建证书后添加路径
                    PlayerSettings.Android.keystorePass = "";
                    PlayerSettings.Android.keyaliasName = "xxx";
                    PlayerSettings.Android.keyaliasPass = "xxx";
                    buildPath = Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}Android");
                    packageName = Path.Combine(buildPath, packageName).Replace('\\', '/');
                    buildOption.target = BuildTarget.Android;
                    buildOption.targetGroup = BuildTargetGroup.Android;
                    packageName += isRelease ? ".aab" : ".apk";
                    buildOption.locationPathName = packageName;
                    EditorUserBuildSettings.androidCreateSymbols = isRelease ? AndroidCreateSymbols.Public : AndroidCreateSymbols.Disabled;
                }
                    break;
                case BuildTarget.iOS:{
#if UNITY_EDITOR_OSX && UNITY_IOS
                    PlayerSettings.iOS.hideHomeButton = true;
                    PlayerSettings.iOS.allowHTTPDownload = true;
					PlayerSettings.iOS.appleDeveloperTeamID = GameConstants.teamID;
					PlayerSettings.iOS.appleEnableAutomaticSigning = isDebug;

                    if (!isDebug)
                    {
                        string provision = isRelease ? GameConstants.app_stroe : GameConstants.ad_hoc;
                        PlayerSettings.applicationIdentifier = GameConstants.bundleID;
                        PlayerSettings.iOS.iOSManualProvisioningProfileID = provision;
                        PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;

                        output?.Add("xcode-team-id", PlayerSettings.iOS.appleDeveloperTeamID);
                        output?.Add("xcode-provision-id", PlayerSettings.iOS.iOSManualProvisioningProfileID);
                        output?.Add("xcode-provision-method", isRelease ? "app-store" : "ad-hoc");
                    }

					buildPath = Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}iOS");
                    buildOption.target = BuildTarget.iOS;
                    buildOption.targetGroup = BuildTargetGroup.iOS;
                    buildOption.locationPathName = buildPath;
                    packageName += ".ipa";
                    packageName = Path.Combine(buildPath, packageName).Replace('\\', '/');
                    break;
#else
                    UnityEngine.Debug.LogError("打IOS包只能在mac上哈，因为需要装扩展包，很多Windows的机器上没装，上面那段代码会报错！");
                    return false;
#endif
                }
                case BuildTarget.StandaloneLinux64:
                    //Linux的Graphic只走Vulkan
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64, false);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneLinux64, new GraphicsDeviceType[]{ GraphicsDeviceType.Vulkan });
                }
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1); //2 = Universal 32-bit ,1 = 64-bit = 0
                    buildPath                    =  Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}Linux");
                    packageName                  =  Path.Combine(buildPath, packageName).Replace('\\', '/');
                    buildOption.target           =  BuildTarget.StandaloneLinux64;
                    buildOption.targetGroup      =  BuildTargetGroup.Standalone;
                    packageName                  += ".bin";
                    buildOption.locationPathName =  packageName;
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    //Windows走默认即可
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64, true);
                }

                    //TODO:这个看资源方面是怎么处理的,资源有额外的Compile这里就需要跳过
                    // PlayerSettings.gpuSkinning = true;
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1); //2 = Universal 32-bit ,1 = 64-bit = 0
                    buildPath                    =  Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}Windows");
                    packageName                  =  Path.Combine(buildPath, packageName).Replace('\\', '/');
                    buildOption.target           =  BuildTarget.StandaloneWindows64;
                    buildOption.targetGroup      =  BuildTargetGroup.Standalone;
                    packageName                  += ".exe";
                    buildOption.locationPathName =  packageName;
                    break;
                case BuildTarget.Switch:
                    PlayerSettings.Switch.useNewStyleFilepaths = isRelease;
                    EditorUserBuildSettings.buildAppBundle = isRelease;
                    buildPath = Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}Switch");
                    packageName = Path.Combine(buildPath, packageName).Replace('\\', '/');
                    buildOption.target = BuildTarget.Switch;
                    buildOption.targetGroup = BuildTargetGroup.Switch;
                    packageName += isRelease ? "" : ""; //TODO:忘了后缀了，想起来添加上(npa)
                    buildOption.locationPathName = packageName;
                    EditorUserBuildSettings.switchCreateRomFile = isRelease;
                    break;
                case BuildTarget.PS4:
                case BuildTarget.PS5:
                    PlayerSettings.PS4.category               = PlayerSettings.PS4.PS4AppCategory.Application;
                    EditorUserBuildSettings.buildAppBundle    = isRelease;
                    buildPath                                 = Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}PSX");
                    packageName                               = Path.Combine(buildPath, packageName).Replace('\\', '/');
                    buildOption.target                        = BuildTarget.PS4;
                    buildOption.targetGroup                   = BuildTargetGroup.PS4;
                    packageName                               += isRelease ? "" : ""; //TODO:忘了后缀了，想起来添加上(npa)
                    buildOption.locationPathName              = packageName;
                    EditorUserBuildSettings.ps4HardwareTarget = PS4HardwareTarget.ProAndBase;
                    break;
                default:
                    UnityEngine.Debug.LogError($"打安装包：当前打包流程本不支持此平台，需要的话联系程序加一下:{EditorUserBuildSettings.activeBuildTarget}");
                    return false;
            }

            output?.Add("build", arg);
            output?.Add("app-name", PlayerSettings.productName);
            output?.Add("bundle-id", PlayerSettings.applicationIdentifier);
            output?.Add("package", packageName);
            output?.Add("build-path", buildPath);
            if (Directory.Exists(buildPath)){
                Directory.Delete(buildPath, true);
            }

            Directory.CreateDirectory(buildPath);
            buildOption.scenes = GetScenes();

        #region 包体Options处理

            Il2CppCompilerConfiguration compilerConfiguration = Il2CppCompilerConfiguration.Release;
            StackTraceLogType           stackTraceLogType     = StackTraceLogType.None;
            switch (_buildType){
                case BuildType.DebugDevelopment:
                    buildOption.options   = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
                    compilerConfiguration = Il2CppCompilerConfiguration.Debug;
                    // stackTraceLogType     = StackTraceLogType.Full;
                    stackTraceLogType     = StackTraceLogType.ScriptOnly;//Script就行了,Full信息太杂了
                    break;
                case BuildType.Debug:
                    buildOption.options   = BuildOptions.None;
                    compilerConfiguration = Il2CppCompilerConfiguration.Release;
                    stackTraceLogType     = StackTraceLogType.ScriptOnly;
                    break;
                case BuildType.Release:
                    buildOption.options   = BuildOptions.None;
                    compilerConfiguration = Il2CppCompilerConfiguration.Master;
                    stackTraceLogType     = StackTraceLogType.None;
                    break;
            }

            //设置c++编译规格
            UnityEditor.PlayerSettings.SetIl2CppCompilerConfiguration(buildOption.targetGroup, compilerConfiguration);
            //设置日志规格
            UnityEditor.PlayerSettings.SetStackTraceLogType(LogType.Error, stackTraceLogType);
            UnityEditor.PlayerSettings.SetStackTraceLogType(LogType.Warning, stackTraceLogType);
            UnityEditor.PlayerSettings.SetStackTraceLogType(LogType.Assert, stackTraceLogType);
            UnityEditor.PlayerSettings.SetStackTraceLogType(LogType.Log, stackTraceLogType);
            UnityEditor.PlayerSettings.SetStackTraceLogType(LogType.Exception, stackTraceLogType);
			PlayerSettings.usePlayerLog = _buildType != BuildType.Release;//先设置logType再设置UsePlayerSetting

        #endregion

            var report = BuildPipeline.BuildPlayer(buildOption);
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android){
                RenameObb(buildPath);
            }

			switch (BuildingCmdEditorConfig.Ins.AutoArchive) {
				case ArchivePkgType.None:
					break;
				case ArchivePkgType.PkgDoNotShip:
					string pkgNameTmp = BuildUtils.GetPkgFolder(arg);
                	FilterZipDebugDirectories(buildPath, pkgNameTmp);
					break;
				case ArchivePkgType.Total:
					string pkgName = BuildUtils.GetPkgFolder(arg);
                	FilterZipDebugDirectories(buildPath, pkgName);
                	ZipMainPkg(buildPath, pkgName);
					break;
			}

            if (BuildingCmdEditorConfig.Ins.PackingEndDirectFolder){
                EditorUtility.RevealInFinder(buildPath);
            }

            UnityEngine.Debug.Log($"[BuildingCmdBuildApp] 包体构建完成!".GreenColor());
            return true;
        }

        private string[] GetScenes(){
            // AddScenes();
            return new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes).FindAll(x => x.enabled).
                                                                                  ConvertAll(x => x.path).
                                                                                  FindAll(x => !string.IsNullOrEmpty(x)).
                                                                                  ToArray();
        }

        private void RenameObb(string path){
            var files = Directory.EnumerateFiles(path);
            foreach (var file in files){
                if (file.EndsWith(".obb")){
                    var newName = $"main.{PlayerSettings.Android.bundleVersionCode}.{PlayerSettings.applicationIdentifier}.obb";
                    var oldName = Path.GetFileName(file);
                    if (newName != oldName){
                        File.Move(file, Path.Combine(path, newName));
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 压缩主包
        /// </summary>
        private void ZipMainPkg(string dir, string zipName){
            string parentDir  = Path.GetDirectoryName(dir);
            string outputPath = $"{parentDir}\\{zipName}";
            //
            EditorUtility.DisplayProgressBar("压缩[主包]文件!", "执行中...(递归执行这里是假的进度条,10g预计25min左右...)", 0.1f);
            bool   rel    = ZipUtils.Zip(dir, outputPath + ".zip", 9, string.Empty);
            string relStr = rel ? "成功".GreenColor() : "失败".RedColor();
            string logStr = $"压缩结果:{relStr}";
            UnityEngine.Debug.Log(logStr);
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 提取Burst与Debug文件夹单独压缩
        /// </summary>
        private void FilterZipDebugDirectories(string buildPath, string packageName){
            const string BURST_END  = "BurstDebugInformation_DoNotShip";
            const string BACKUP_END = "BackUpThisFolder_ButDontShipItWithYourGame";
            //筛选backup文件夹
            string[]     files       = Directory.GetDirectories(buildPath, "*", SearchOption.AllDirectories);
            List<string> needPackDir = new List<string>();
            int          len         = files?.Length ?? 0;
            for (int i = 0; i < len; ++i){
                var low = files[i].ToLower();
                if (files[i].EndsWith(BURST_END) || files[i].EndsWith(BACKUP_END)){
                    needPackDir.Add(files[i]);
                }
            }

            //移动文件目录
            string createDirPath = $"{buildPath}\\..\\{packageName}_backup";
            var    dirInfo       = Directory.CreateDirectory(createDirPath);
            len = needPackDir.Count;
            for (int i = 0; i < len; ++i){
                string packDir     = needPackDir[i];
                string dirName     = Path.GetDirectoryName(packDir);
                string dirFileName = packDir.Substring(dirName.Length);
                string targetPath  = $"{createDirPath}\\{dirFileName}";
                Directory.Move(packDir, targetPath);
            }

            //打包Backup目录
            EditorUtility.DisplayProgressBar("压缩[备份]文件!", "执行中...", 0.1f);
            bool   rel    = ZipUtils.Zip(createDirPath, createDirPath + ".zip", 9, string.Empty);
            string relStr = rel ? "成功".GreenColor() : "失败".RedColor();
            string logStr = $"压缩结果:{relStr}";
            UnityEngine.Debug.Log(logStr);
            EditorUtility.ClearProgressBar();
            //压缩结束,删除不需要目录
            Directory.Delete(createDirPath, true);
        }
    }
}
