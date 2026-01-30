using System;

namespace EscapeGame.Building{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandPostAttribute : Attribute{
        public CommandOperationType CommandType{ get; private set; }
        public CommandPostAttribute(CommandOperationType commandType){ this.CommandType = commandType; }
    }
}