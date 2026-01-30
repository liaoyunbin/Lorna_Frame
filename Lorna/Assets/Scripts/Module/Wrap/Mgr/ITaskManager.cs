
using System;

namespace LornaGame.ModuleExtensions
{
    public interface ITaskManager : IMgr
    {
        void DoTask(Action action, int millisecondsDelay);
    }
}
