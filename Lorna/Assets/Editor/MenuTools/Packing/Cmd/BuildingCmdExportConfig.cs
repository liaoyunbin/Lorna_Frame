using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LornaGame.ModuleExtensions;

namespace EscapeGame.Building{
    [CommandPkgStr("cfg", "export-cfg", true)]
    public class BuildingCmdExportConfig : IBuildingCommand{
        public string                     Name          => "导出配置表";
        public string                     Help          => null;
        public Action<string>             OnEditorExtra => null;
        public BuildingCmdValueType       CmdValType    => BuildingCmdValueType.Choices;
        public Dictionary<string, string> Choices       { get; } = new(){ { "export", "导出所有表格" },{ "export-language", "导出多语言表格" }, };

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
            var asm = AppDomain.CurrentDomain.GetAssemblies().First(t => t.GetName().Name == "Assembly-CSharp-Editor");
            if (null == asm){
                return false;
            }

            var type = asm.GetTypes().First(t => t.FullName == "LornaGame.Editor.DesignerMenuTools");
            if (null == type){
                return false;
            }

            string functionName = string.Empty;
            switch (arg){
                case "export":          functionName = "ExportData_ALL_Silence_NOTPost"; break;
                case "export-language": functionName = "ExportData_Language_Silence_NOTPost"; break;
            }

            if (string.IsNullOrEmpty(functionName)){
                return false;
            }

            var exportFunc = type.GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic);
            if (exportFunc != null){ exportFunc.Invoke(null, null); }

            UnityEngine.Debug.Log($"[BuildingCmdExportConfig] 配置表导出完成!".GreenColor());
            return true;
        }
    }
}