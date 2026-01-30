using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "TextureProcessorSettings", menuName = "Tools/Texture Processor Settings")]
public class TextureProcessorSettings : ScriptableObject
{
    // 存储所有需要处理的文件夹路径
    public List<string> targetFolders = new List<string>();

    // --- 单例模式加载 ---
    // 提供一个静态的、方便的方法来获取项目中的设置文件
    private static TextureProcessorSettings s_instance;
    public static TextureProcessorSettings Instance
    {
        get
        {
            if (s_instance == null)
            {
                // 尝试在项目中查找这个SO文件
                string[] guids = AssetDatabase.FindAssets("t:TextureProcessorSettings");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    s_instance = AssetDatabase.LoadAssetAtPath<TextureProcessorSettings>(path);
                }
                else
                {
                    // 如果找不到，可以在控制台给一个提示
                    Debug.LogWarning("未找到TextureProcessorSettings文件，自动处理将不会生效。请在项目中创建一个。");
                }
            }
            return s_instance;
        }
    }
}