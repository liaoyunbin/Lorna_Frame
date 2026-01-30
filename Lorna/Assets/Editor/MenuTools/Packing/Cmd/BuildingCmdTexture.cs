using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EscapeGame.Building{
    ////TODO:暂定版本管理规则，当前没考虑地区
    [CommandPkgStr(shortCommand : "tex", fullCommand : "export-texture")]
    public class BuildingCmdTexture : IBuildingCommand{
        public string                     Name          => "批处理Texture";
        public string                     Help          => "Resources目录下批处理Texture【开启纹理的SRGB.关闭纹理的mipmap】";
        public BuildingCmdValueType       CmdValType    => BuildingCmdValueType.None;
        public Dictionary<string, string> Choices       => null; //key ： 标记， value 说明 
        public Action<string>             OnEditorExtra => null;

        public async Task<bool >OnExecute(string arg, Dictionary<string, string> output){
            TextureProcessorLogic.RunManualBatchProcess();
            return true;
        }
    }
}