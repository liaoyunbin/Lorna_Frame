using System;

namespace EscapeGame.Building{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class InsConfigAttribute : Attribute{
        public string JsonPath{ get; private set; }
        public InsConfigAttribute(string path){ this.JsonPath = path; }
    }
}