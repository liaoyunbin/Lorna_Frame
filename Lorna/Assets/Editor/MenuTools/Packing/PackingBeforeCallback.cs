using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace EscapeGame.Building{
    public class PackingBeforeCallback : IPreprocessBuildWithReport{
        public int  callbackOrder                        { get{ return 0; } }
        public void OnPreprocessBuild(BuildReport report){ }
    }
}