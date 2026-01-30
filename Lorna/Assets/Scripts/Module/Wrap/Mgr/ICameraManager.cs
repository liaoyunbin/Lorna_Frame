
namespace LornaGame.ModuleExtensions
{
    public interface ICameraManager: IMgr
    {
        UnityEngine.Camera GetUICamera();
        UnityEngine.Camera GetMainCamera();
    }
}
