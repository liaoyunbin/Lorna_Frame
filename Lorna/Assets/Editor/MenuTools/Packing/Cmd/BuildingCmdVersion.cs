using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EscapeGame.Building.Config;
using EscapeGame.Utils;
using Sirenix.Utilities;
using UnityEditor;

namespace EscapeGame.Building{
    ////TODO:暂定版本管理规则，当前没考虑地区
    [CommandPkgStr(shortCommand : "v", fullCommand : "version")]
    public class BuildingCmdVersion : IBuildingCommand{
        public string                     Name          => "设置版本号";
        public string                     Help          => "最后一段表示构建号，\n  eg : 1.0.1.23\n    23 为构建号\n    1.0.1 为APP版本号";
        public BuildingCmdValueType       CmdValType    => BuildingCmdValueType.StringVal;
        public Dictionary<string, string> Choices       => null; //key ： 标记， value 说明 
        public Action<string>             OnEditorExtra => null;

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
            //每次出包，资源版本号都是1，后面更新都再加1
            //TODO:当前没考虑地区,有地域需求的打包则额外考虑,初步设想是分地域出包则把美术内容分单独一个仓库，多地域则多地域分支，每次打包按出包美术资源额外building,然后接入到这个主building内
            var mainCfg = GameConfig.Instance;
            EditorUtility.SetDirty(mainCfg);
            var     lastVersion = new Version(mainCfg.AppVer);
            Version cmdVersion  = null;
            cmdVersion = false == string.IsNullOrWhiteSpace(arg) ? new Version(arg) : new Version(lastVersion.Major, lastVersion.Minor, lastVersion.Revision);
            int buildNumber = lastVersion.Build + 1;
            if (cmdVersion.Build > 0){
                buildNumber = cmdVersion.Build;
            }
            else if (File.Exists(GetBuildNumberFilePath())){
                if (int.TryParse(File.ReadAllText(GetBuildNumberFilePath()), out int oldBuildNumber)){
                    buildNumber = oldBuildNumber + 1;
                }
            }

            string appVersion = $"{cmdVersion.Major}.{cmdVersion.Minor}.{cmdVersion.Revision}";
            mainCfg.AppVer               = $"{cmdVersion.Major}.{cmdVersion.Minor}.{cmdVersion.Revision}.{buildNumber}";
            PlayerSettings.bundleVersion = appVersion;
            //当前BuildNumber只有移动端有,因为是直接对应的XCode与Android内部构建的BuildNumber
            switch (EditorUserBuildSettings.activeBuildTarget){
                case BuildTarget.Android: PlayerSettings.Android.bundleVersionCode = buildNumber; break;
                case BuildTarget.iOS:     PlayerSettings.iOS.buildNumber           = buildNumber.ToString(); break;
                // case BuildTarget.Switch : PlayerSettings.Switch.buildNumber        = buildNumber.ToString(); break;
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    // PlayerSettings.Windows.buildNumber        = buildNumber.ToString();
                    break;
                // case BuildTarget.PS4:
                // case BuildTarget.PS5: PlayerSettings.PS4.buildNumber        = buildNumber.ToString(); break;
                default:
                    UnityEngine.Debug.LogError($"当前打包流程本不支持此平台，需要的话联系程序加一下:{EditorUserBuildSettings.activeBuildTarget}");
                    return false;
            }

            string fullCmd = this.GetType().GetCustomAttribute<CommandPkgStrAttribute>()?.FullCommand;
            output.Add(fullCmd, mainCfg.AppVer);
            string writeFilePath = GetBuildNumberFilePath();
            File.WriteAllText(writeFilePath, buildNumber.ToString());
            return true;
        }

        /// <summary>
        /// 为了方便快速查看,直接创建一个文件给文件夹查看
        /// </summary>
        private string GetBuildNumberFilePath(){
            var    target = EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();
            string rel    = string.Format(PackingConfIns.Ins.PackingBuildNumber, target);
            //Check dir
            string dir = Path.GetDirectoryName(rel);
            if (false == Directory.Exists(dir)){
                Directory.CreateDirectory(dir);
            }

            //
            return rel;
        }
    }
}