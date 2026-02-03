using LornaGame.ModuleExtensions;
using UnityEngine;
namespace LornaGame.Runtime
{
    public class LogsTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GenCoreComp<LogAnalyzer>("LogAnalyzer", null);
            LogAnalyzer.Instance.Setup(UnityEngine.Debug.unityLogger.logEnabled);
        }

        private void GenCoreComp<T>(string name, GameObject _parent, bool dontDestroy = true) where T : Component
        {
            var go = new GameObject(name);
            go.AddComponent<T>();
            if (dontDestroy)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            go.transform.SetParent(_parent.transform);
        }
    }
}
