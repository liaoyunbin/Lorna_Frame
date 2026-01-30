namespace LornaGame.UIExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    //转为UTF8格式：
    /// <summary>
    /// 参考资料
    /// https://github.com/onelei/Lemon/blob/master/Lemon.Framework/Assets/LemonFramework/Editor/UI/UIBase_Generate.cs#L24
    /// </summary>
    public class UIGeneralCode
    {
#if UNITY_EDITOR
        private const string KEY_VARIABLE = "//==自动化变量开始";
        private const string KEY_PATH = "//==自动化路径开始";

        private static string UI_TEMPLATE_PATH
        {
            get
            {
                return Application.dataPath + "/Scripts/FrameWorkUI/UITemplate/View/UITemplate.cs";
            }
        }
        private static string UI_TEMPLATE_LOGIC_PATH
        {
            get
            {
                return Application.dataPath + "/Scripts/FrameWorkUI/UITemplate/View/UITemplate_Logic.cs";
            }
        }
        private static string UI_PANEL_CTRL_TEMPLATE_PATH
        {
            get
            {
                return Application.dataPath + "/Scripts/FrameWorkUI/UITemplate/Ctrl/UIPanelCtrlTemplate.cs";
            }
        }
        private static string UI_PANEL_CTRL_GENERATE_PATH
        {
            get
            {
                return Application.dataPath + "/Scripts/Runtime/UI/Ctrl/";
            }
        }
        /// <summary>
        ///  tag页签对应的unity组件
        /// </summary>
        private static Dictionary<string, Type> name_type = new Dictionary<string, Type>()
        {
            {"UI/Unity/Button", typeof(Button)},
            {"UI/Unity/Image", typeof(Image)},
            {"UI/Unity/Slider", typeof(Slider)},
            {"UI/Unity/Text", typeof(Text)},
        };

        /// <summary>
        /// key: tag   value: （文件路径，继承基类）
        /// </summary>
        private static Dictionary<string, (string, string)> name_directoryPath = new Dictionary<string, (string, string)>()
        {
            {"UI/View/Panel",       new (Application.dataPath + "/Scripts/Runtime/UI/View/Panel/",       "UI_Panel")},
            {"UI/View/SubPanel",    new (Application.dataPath + "/Scripts/Runtime/UI/View/SubPanel/",    "UI_SubPanel")},
            {"UI/View/Component",   new (Application.dataPath + "/Scripts/Runtime/UI/View/Component/",   "UI_Base")},
        };

        /// <summary>
        /// key:预设体  value:type类型
        /// </summary>
        private static Dictionary<Transform, string> transformGroup = new Dictionary<Transform, string>();
        private static StringBuilder stringBuilder = new StringBuilder(1024);
        /// <summary>
        /// 根据UI面板自动生成组件
        /// </summary>

        public static void GeneralUIView(GameObject CacheGameObject)
        {
            string directoryPath = name_directoryPath[CacheGameObject.tag].Item1;
            string inheritScriptName = name_directoryPath[CacheGameObject.tag].Item2;
            string scriptName = string.Format("{0}_Design", CacheGameObject.name);
            var ClassText = GetTemplateText(CacheGameObject, scriptName, inheritScriptName);
            Write(directoryPath, scriptName, ClassText);
        }

        /// <summary>
        /// 已有对应logic代码时不做重新生成
        /// </summary>
        /// <param name="CacheGameObject"></param>
        public static void GeneralUIView_Logic(GameObject CacheGameObject)
        {
            //生成
            string directoryPath = name_directoryPath[CacheGameObject.tag].Item1;
            string inheritScriptName = name_directoryPath[CacheGameObject.tag].Item2;
            string scriptName = string.Format("{0}_Logic", CacheGameObject.name);
            // 查找脚本
            MonoScript script = FindScriptByName(scriptName);
            if (script != null)
            {
                return;
            }
            var ClassText = GetTemplate_Logic_Text(CacheGameObject, scriptName, inheritScriptName);
            Write(directoryPath, scriptName, ClassText);
        }

        /// <summary>
        /// 已有对应ctrl代码时不做重新生成
        /// </summary>
        /// <param name="CacheGameObject"></param>
        public static void GeneralUIPanelCtrl(GameObject CacheGameObject)
        {
            //生成
            string directoryPath = UI_PANEL_CTRL_GENERATE_PATH;
            string ctrlName = string.Format("{0}_PanelCtrl", CacheGameObject.name);
            string monoName = string.Format("{0}_Design", CacheGameObject.name);
            // 查找脚本
            //MonoScript script = FindScriptByName(ctrlName);
            //if (script != null)
            //{
            //    return;
            //}
            var ClassText = GetPanelTemplateCtrl_Text(CacheGameObject, ctrlName, monoName);
            Write(directoryPath, ctrlName, ClassText);
        }

        /// <summary>
        /// UI_TEMPLATE_PATH 作为模板的最终文本
        /// </summary>
        /// <returns></returns>
        private static string GetTemplateText(GameObject CacheGameObject, string scriptName, string inheritScriptName)
        {
            //读取
            StreamReader streamReader = new StreamReader(UI_TEMPLATE_PATH, Encoding.UTF8);
            string ClassText = streamReader.ReadToEnd();
            streamReader.Close();

            transformGroup.Clear();
            Transform[] children = CacheGameObject.transform.GetComponentsInChildren<Transform>(true);
            string childName = string.Empty;
            for (int i = 0; i < children.Length; i++)
            {
                Transform child = children[i];
                if (child == CacheGameObject.transform)
                {
                    continue;
                }

                bool flag = false;
                string fullTage = child.tag;
                if (fullTage.Contains("UI/View"))
                {
                    transformGroup.Add(child, string.Format("{0}_Design", child.name));
                    flag = true;
                }
                else if (fullTage.Contains("UI/Unity"))
                {
                    if (name_type.TryGetValue(fullTage, out var type))
                    {
                        transformGroup.Add(child, type.Name);
                        flag = true;
                    }
                }

                if (flag)
                {
                    childName = child.name.Replace("（", "_").Replace("）", "_").Replace("(", "_").Replace(")", "_").Replace(" ", "");
                    child.name = childName;
                }
            }


            //添加自动化的变量
            stringBuilder.Clear();
            stringBuilder.Append("\n");
            var enumerator = transformGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string name = enumerator.Current.Key.name;
                string typeName = enumerator.Current.Value;
                stringBuilder.Append("        public " + typeName + " " + name + "; ");
                stringBuilder.Append("\n");
            }


            ClassText = ClassText.Replace("UITemplate", scriptName);
            ClassText = ClassText.Replace("UI_Base", inheritScriptName);
            ClassText = ClassText.Replace(KEY_VARIABLE, KEY_VARIABLE + stringBuilder.ToString());

            //添加自动化的变量路径
            stringBuilder.Clear();
            stringBuilder.Append("\n");
            enumerator = transformGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Transform childTransform = enumerator.Current.Key;
                string typeName = enumerator.Current.Value;
                stringBuilder.Append("            " + childTransform.name + " = " + "CacheTransform.Find(\"" + GetPath(childTransform, CacheGameObject.transform) + "\").GetComponent<" + typeName + ">();\n");
            }

            ClassText = ClassText.Replace(KEY_PATH, KEY_PATH + stringBuilder.ToString());
            return ClassText;
        }

        /// <summary>
        /// UI_TEMPLATE_LOGIC_PATH 作为模板的最终文本
        /// </summary>
        /// <returns></returns>
        private static string GetTemplate_Logic_Text(GameObject CacheGameObject, string scriptName, string inheritScriptName)
        {
            //读取
            StreamReader streamReader = new StreamReader(UI_TEMPLATE_LOGIC_PATH, Encoding.UTF8);
            string ClassText = streamReader.ReadToEnd();
            streamReader.Close();
            ClassText = ClassText.Replace("UITemplate", scriptName.Replace("_Logic", "_Design"));
            return ClassText;
        }


        /// <summary>
        /// UI_PANEL_CTRL_TEMPLATE_PATH 作为模板的最终文本
        /// </summary>
        /// <returns></returns>
        private static string GetPanelTemplateCtrl_Text(GameObject CacheGameObject, string ctrlName,string monoName)
        {
            //读取
            StreamReader streamReader = new StreamReader(UI_PANEL_CTRL_TEMPLATE_PATH, Encoding.UTF8);
            string ClassText = streamReader.ReadToEnd();
            streamReader.Close();
            ClassText = ClassText.Replace("UIPanelCtrlTemplate", ctrlName);
            ClassText = ClassText.Replace("UI_Panel", monoName);
            return ClassText;
        }
        private static void Write(string directoryPath, string scriptName, string ClassText)
        {
            // 确保目录存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string uiBasePath = string.Format(directoryPath + "/{0}.cs", scriptName);
            StreamWriter streamWriter = new StreamWriter(uiBasePath, false, Encoding.UTF8);
            streamWriter.Write(ClassText);
            streamWriter.Close();
            UnityEditor.AssetDatabase.Refresh();
        }
        /// <summary>
        /// 对当前组件进行脚本绑定
        /// </summary>
        /// <param name="CacheGameObject"></param>
        public static void BindUIView(GameObject CacheGameObject)
        {
            var temp = CacheGameObject.transform.GetComponent<UI_Base>();
            if (temp == null)
            {
                string scriptName = string.Format("{0}_Design", CacheGameObject.name);
                AttachScriptToPrefab(CacheGameObject, scriptName);
            }


            temp = CacheGameObject.transform.GetOrAddComponent<UI_Base>();
            temp.GeneratePath();

            // 保存
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssetIfDirty(CacheGameObject);
        }

        private static string GetPath(Transform child, Transform parent)
        {
            if (child == null || parent == null || child == parent)
                return string.Empty;

            Transform tmp = child;
            string path = tmp.name;
            while (tmp.parent != parent)
            {
                tmp = tmp.parent;
                path = string.Concat(tmp.name, "/", path);
            }
            return path;
        }

        private static void AttachScriptToPrefab(GameObject selectedPrefab, string scriptName)
        {
            // 查找脚本
            MonoScript script = FindScriptByName(scriptName);
            if (script == null)
            {
                Debug.LogError($"未找到脚本: {scriptName}");
                return;
            }

            // 获取脚本类型
            Type scriptType = script.GetClass();
            if (scriptType == null)
            {
                Debug.LogError($"脚本 {scriptName} 没有对应的类，或类名与文件名不一致");
                return;
            }
            Debug.Log(scriptType);
            // 检查是否已经挂载了该脚本
            if (selectedPrefab.GetComponent(scriptType) != null)
            {
                Debug.LogWarning($"预设体已挂载脚本: {scriptName}");
                return;
            }

            // 挂载脚本
            selectedPrefab.AddComponent(scriptType);
            Debug.Log($"挂载脚本 {scriptName} 成功");
        }

        // 根据脚本名查找MonoScript
        private static MonoScript FindScriptByName(string scriptName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Script {scriptName}");
            if (guids.Length == 0)
            {
                return null;
            }

            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
        }
#endif
    }
}
