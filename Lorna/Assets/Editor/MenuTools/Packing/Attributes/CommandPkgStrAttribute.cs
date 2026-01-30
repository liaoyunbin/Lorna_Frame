using System;

namespace EscapeGame.Building{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class CommandPkgStrAttribute : Attribute{
        public string ShortCommand{ get; private set; }
        public string FullCommand { get; private set; }
        public bool   WaitCompile { get; private set; }

        public CommandPkgStrAttribute(string shortCommand, string fullCommand, bool waitCompile = false){
            this.ShortCommand = shortCommand;
            this.FullCommand  = fullCommand;
            this.WaitCompile  = waitCompile;
        }

        public CommandPkgStrAttribute(string shortCommand){
            this.ShortCommand = shortCommand;
            this.FullCommand  = string.Empty;
            this.WaitCompile  = false;
        }
    }
}