namespace FrameWork_UI
{
    using UnityEditor;
    using UnityEngine;


    //转为UTF8格式：
    public class UITools
    {
        //[MenuItem("Assets/生成UIView代码", false, 100)]
        [MenuItem("GameObject/代码生成/1、生成UIView代码", false, -100)]
        private static void GeneralUIView()
        {
            // 获取当前选中的预设体文件
            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
            {
                return;
            }
            //自动生成脚本
            UIGeneralCode.GeneralUIView(selectedPrefab);
            UIGeneralCode.GeneralUIView_Logic(selectedPrefab);
        }

        [MenuItem("GameObject/代码生成/2、绑定UIView代码", false, -100)]
        private static void BindUIView()
        {
            // 获取当前选中的预设体文件
            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
            {
                return;
            }
            //自动绑定
            UIGeneralCode.BindUIView(selectedPrefab);
        }

        [MenuItem("GameObject/代码生成/3、生成UIPanelCtrl代码", false, -100)]
        private static void GeneralUICtrl()
        {
            // 获取选中的游戏对象（可能是预设体实例）
            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
            {
                return;
            }
            //自动生成脚本
            UIGeneralCode.GeneralUIPanelCtrl(selectedPrefab);
        }

        //是预设体才显示此提示

        [MenuItem("GameObject/代码生成/生成UIView代码", true)]
        static bool ValidateGeneralUIView()
        {
            GameObject selected = Selection.activeGameObject;
            return selected != null && !PrefabUtility.IsPartOfAnyPrefab(selected) && selected.tag.Contains("UI/View");
        }


        [MenuItem("GameObject/代码生成/3、生成UIPanelCtrl代码", true)]
        static bool ValidateGeneralUICtrl()
        {
            GameObject selected = Selection.activeGameObject;
            return selected != null &&
                !PrefabUtility.IsPartOfAnyPrefab(selected) &&
                selected.tag.Contains("UI/View/Panel");
        }
    }
}
