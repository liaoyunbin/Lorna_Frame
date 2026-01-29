#if UNITY_EDITOR

using ParadoxNotion.Design;
using UnityEngine;
using NodeCanvas.DialogueTrees;
using Sirenix.Utilities;

namespace NodeCanvas.Editor
{

    ///<summary>A drawer for dialogue tree statements</summary>
    public class StatementDrawer : ObjectDrawer<Statement>
    {
        public override Statement OnGUI(GUIContent content, Statement instance) {
            if ( instance == null ) { instance = new Statement("..."); }

            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("[多语言Key]: ", UnityEditor.EditorStyles.label,GUILayout.Width(70));
            instance.lanKey = UnityEditor.EditorGUILayout.TextArea(instance.lanKey, Styles.wrapTextArea, GUILayout.Height(20));
            UnityEditor.EditorGUILayout.EndHorizontal();
            instance.text     = UnityEditor.EditorGUILayout.TextArea(instance.text,   Styles.wrapTextArea, GUILayout.Height(100));
            instance.audio    = UnityEditor.EditorGUILayout.ObjectField("音频资源", instance.audio, typeof(AudioClip), false) as AudioClip;
            instance.image    = UnityEditor.EditorGUILayout.ObjectField("头像",   instance.image, typeof(Sprite),    false) as Sprite;
            instance.flip    = UnityEditor.EditorGUILayout.Toggle("头像翻转",   instance.flip);
            instance.Type     = (StatementType)UnityEditor.EditorGUILayout.EnumPopup("对话节点类型", instance.Type);
            instance.Flow     = UnityEditor.EditorGUILayout.CurveField("播放曲线", instance.Flow);
            instance.Duration = UnityEditor.EditorGUILayout.Slider("本节点播放时长",instance.Duration,0f,10f);
            instance.meta     = UnityEditor.EditorGUILayout.TextField("Metadata", instance.meta);
            FixedMultipleLan(instance);
            return instance;
        }

        private void FixedMultipleLan(Statement instance){
            GUILayout.BeginVertical();
            if (GUILayout.Button("[修补Key]")){
                instance.lanKey = NodeCanvas.Editor.NodeCanvasInternalWrapTool.GetLangKey(Editor.NodeCanvasInternalWrapTool.Language,instance.text);
            }

            GUILayout.EndVertical();
        }
    }
}

#endif