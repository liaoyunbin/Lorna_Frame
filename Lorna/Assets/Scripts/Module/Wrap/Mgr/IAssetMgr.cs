
using System;

namespace LornaGame.ModuleExtensions
{
    public interface  IAssetsManager: IMgr
    {
        T LoadAsset<T>(string location) where T : UnityEngine.Object;
        UnityEngine.Object LoadAsset(string location, Type type);
    }
}
