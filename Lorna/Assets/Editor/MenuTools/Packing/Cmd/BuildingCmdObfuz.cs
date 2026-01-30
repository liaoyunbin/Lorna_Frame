//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using LornaGame.ModuleExtensions;
//using Obfuz.Unity;

//namespace EscapeGame.Building{
//    [CommandPkgStr(shortCommand : "o", fullCommand : "obfuz")]
//    public class BuildingCmdObfuz : IBuildingCommand{
//        public string                     Name          => "代码混淆与加密";
//        public string                     Help          => "设置代码混淆与加密,混淆后的key放入到Resource/Obfuz/defaultStaticSecretKey内";
//        public BuildingCmdValueType       CmdValType    => BuildingCmdValueType.None;
//        public Dictionary<string, string> Choices       => null;
//        public Action<string>             OnEditorExtra => null;

//        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
//            ObfuzMenu.GenerateEncryptionVM();
//            ObfuzMenu.SaveSecretFile();
//            UnityEngine.Debug.Log($"[BuildingCmdObfuz] 代码混淆完成!".GreenColor());
//            return true;
//        }
//    }
//}