using System.Collections.Generic;
using System.Threading.Tasks;

namespace EscapeGame.Building{
    public interface IBuildingCommand{
        string                     Name         { get; }
        string                     Help         { get; }
        BuildingCmdValueType       CmdValType   { get; }
        Dictionary<string, string> Choices      { get; } //key ： 标记， value 说明 
        System.Action<string>      OnEditorExtra{ get; }
        Task<bool>                 OnExecute(string arg, Dictionary<string, string> output);
    }
}