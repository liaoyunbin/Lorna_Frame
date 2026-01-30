using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using LornaGame.ModuleExtensions;

namespace EscapeGame.Building{
    [CommandPkgStr("p", "platform", true)]
    public class BuildingCmdPlatform : IBuildingCommand{
        public string                Name          => "切平台";
        public string                Help          => "在打包之前，确认平台这一步是必填的";
        public Action<string> OnEditorExtra => null;
        public BuildingCmdValueType  CmdValType    => BuildingCmdValueType.Choices;

        public Dictionary<string, string> Choices{ get; } = new Dictionary<string, string>{
                                                                                              { "and", "切换到Android平台" },
                                                                                              { "ios", "切换到iOS平台" },
                                                                                              { "win", "切换到Windows64平台" },
                                                                                              { "mac", "切换到OSX" },
                                                                                              { "swi", "切换到Switch" },
                                                                                              { "psx", "切换到PlayStation" },
                                                                                          };

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
            switch (arg){
                case "android":
                case "and":
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    }

                    break;
                case "ios":
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                    }

                    break;
                case "win":
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    }

                    break;
                case "mac":
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSX){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
                    }

                    break;
                case "swi":
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Switch){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Switch, BuildTarget.Switch);
                    }

                    break;
                case "psx":
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.PS4){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.PS4, BuildTarget.PS4);
                    }
                    else if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.PS5){
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.PS5, BuildTarget.PS5);
                    }

                    break;
                default:
                    UnityEngine.Debug.LogError($"切平台：当前打包流程不支持此平台:{arg}".RedColor());
                    return false;
            }

            return true;
        }
    }
}