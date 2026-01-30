
namespace FrameWork_Tools
{
    public interface ICameraManager: IMgr
    {
        UnityEngine.Camera GetUICamera();
        UnityEngine.Camera GetMainCamera();
    }
}
