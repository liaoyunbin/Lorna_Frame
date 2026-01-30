
using System;

namespace FrameWork_Tools
{
    public interface  IAssetsManager: IMgr
    {
        T LoadAsset<T>(string location) where T : UnityEngine.Object;
        UnityEngine.Object LoadAsset(string location, Type type);
    }
}
