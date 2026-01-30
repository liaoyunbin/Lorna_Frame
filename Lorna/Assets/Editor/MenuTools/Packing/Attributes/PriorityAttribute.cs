using System;

namespace EscapeGame.Building{
    //TODO:抽到Main adf内，这个属于通用Attribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PriorityAttribute : Attribute{
        public int Priority{ get; private set; }
        public PriorityAttribute(int priority){ this.Priority = priority; }
    }
}