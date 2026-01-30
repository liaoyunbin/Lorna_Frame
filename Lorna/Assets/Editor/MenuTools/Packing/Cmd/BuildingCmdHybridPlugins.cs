using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LornaGame.ModuleExtensions;
using UnityEditor;

namespace EscapeGame.Building
{
    [CommandPkgStr("s", "pluginsSetting", true)]
    public class BuildingCmdHybridPlugins : IBuildingCommand
    {
        public string Name => "热更插件开关";
        public string Help => "热更插件开关";
        public Action<string> OnEditorExtra => null;
        public BuildingCmdValueType CmdValType => BuildingCmdValueType.Choices;

        public Dictionary<string, string> Choices { get; } = new Dictionary<string, string>{
                                                                                              { "open", "开启热更插件" },
                                                                                              { "close", "关闭热更插件" },
                                                                                          };

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output)
        {
            switch (arg)
            {
                case "open":
                    HybridCLR.Editor.Settings.HybridCLRSettings.Instance.enable = true;
                    break;
                case "close":
                    HybridCLR.Editor.Settings.HybridCLRSettings.Instance.enable = false;
                    break;
            }

            return true;
        }
    }
}