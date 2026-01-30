// 文件名: TextureProcessorWindow.cs (新建)
using UnityEditor;
using UnityEngine;

public class TextureProcessorWindow : EditorWindow
{
    private TextureProcessorSettings settings;
    private SerializedObject serializedSettings;
    private SerializedProperty targetFoldersProp;

    [MenuItem("Tools/图片批处理【手动】")]
    public static void ShowWindow()
    {
        GetWindow<TextureProcessorWindow>("Texture Processor");
    }

    private void OnEnable()
    {
        settings = TextureProcessorSettings.Instance;
        if (settings == null)
        {

            return;
        }
        
        serializedSettings = new SerializedObject(settings);
        targetFoldersProp = serializedSettings.FindProperty("targetFolders");
    }

    private void OnGUI()
    {
        if (settings == null)
        {
            EditorGUILayout.HelpBox("未找到TextureProcessorSettings文件。请通过右键菜单 'Create > Tools > Texture Processor Settings' 在项目中创建一个。", MessageType.Error);
            return;
        }
        
        EditorGUILayout.LabelField("图片批处理设定", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("在这里管理需要自动处理纹理格式的文件夹。修改后会自动保存。", MessageType.Info);
        
        // 更新序列化对象
        serializedSettings.Update();

        // 绘制文件夹列表
        EditorGUILayout.PropertyField(targetFoldersProp, true);

        // 应用修改到SO文件
        serializedSettings.ApplyModifiedProperties();

        EditorGUILayout.Space(20);

        // 手动执行按钮
        if (GUILayout.Button("手动执行批量处理", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("确认", "这将对所有配置路径下的纹理应用设置，是否继续？", "是", "否"))
            {
                TextureProcessorLogic.RunManualBatchProcess();
            }
        }
    }
}