
using System;

namespace FrameWork_Tools
{
    public interface ITaskManager : IMgr
    {
        void DoTask(Action action, int millisecondsDelay);
    }
}
