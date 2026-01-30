using System.IO;
using EscapeGame.Building.Config;
using UnityEditor;
using UnityEngine;

namespace EscapeGame.Building{
    public class ResPackingRunner{
        [MenuItem("策划工具/运行首包模式")]
        private static void RunFirstPackMode(){
            EditorApplication.EnterPlaymode();
            //
            FirstPackingBundleState cnf = null;
            if (!File.Exists(BuildingCmdPackABRes.F_RUNNING_FIRST)){
                cnf = new FirstPackingBundleState();
            }
            else{
                string json = File.ReadAllText(BuildingCmdPackABRes.F_RUNNING_FIRST);
                cnf = JsonUtility.FromJson<FirstPackingBundleState>(json);
            }

            cnf.ToggleRunner = true;
            string writeJson = JsonUtility.ToJson(cnf);
            File.WriteAllText(BuildingCmdPackABRes.F_RUNNING_FIRST, writeJson);
        }
    }
}