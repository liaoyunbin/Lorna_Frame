//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using EscapeGame.Utils;

//namespace EscapeGame.Building{
//    [CommandPkgStr(shortCommand : "svc", fullCommand : "shadervariants")]
//    public class BuildCmdPackSVC : IBuildingCommand{
//        private string                     BUNDLE_NAME     => GameConfig.Instance.ResPack;
//        private string                     SVC_OUTPUT_PATH => GameConfig.Instance.ShaderVariantPath;
//        public  string                     Name            => "Build SVC资产";
//        public  string                     Help            => $"构建ShaderVariantsCollection,最好在资源构建后!数量最小1000(保存路径:{SVC_OUTPUT_PATH})";
//        public  BuildingCmdValueType       CmdValType      => BuildingCmdValueType.IntVal; //Shader变体容量,当前1000即可
//        public  Dictionary<string, string> Choices         => null;
//        public  Action<string>             OnEditorExtra   => null;

//        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
//            int capacity = 1000;
//            if (!int.TryParse(arg, out capacity)){
//                return false;
//            }

//            bool relState = false;
//            ShaderVariantCollector.Run(
//                                       SVC_OUTPUT_PATH,
//                                       BUNDLE_NAME,
//                                       capacity,
//                                       () => {
//                                           UnityEngine.Debug.Log("[BuildCmdPackSVC] Shader变体Pack完成".GreenColor());
//                                           relState = true;
//                                       }
//                                      );
//            await Until(() => { return relState; });
//            return relState;
//        }

//        public void Cancel(){
            
//        }

//        protected async Task Until(Func<bool> function){
//            while (!function.Invoke()) await Task.Yield();
//        }
//    }
//}