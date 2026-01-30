using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public static class TextureProcessorLogic
{
public static void RunManualBatchProcess()
    {
        var settings = TextureProcessorSettings.Instance;
        if (settings == null || settings.targetFolders.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "未找到或未配置TextureProcessorSettings文件！", "确定");
            return;
        }
        
        int checkedCount = 0;
        int processedCount = 0; 
        int atlasCheckedCount = 0;
        int atlasProcessedCount = 0;
        
        try
        {
            AssetDatabase.StartAssetEditing();
            
            foreach (var folderPath in settings.targetFolders)
            {
                if (string.IsNullOrEmpty(folderPath)) continue;
                DirectoryInfo direction = new DirectoryInfo(folderPath);
                if (!direction.Exists) continue;
                
                var allFiles = direction.GetFiles("*.*", SearchOption.AllDirectories)
                    .Where(f => f.Extension.Equals(".png", System.StringComparison.OrdinalIgnoreCase) ||
                                f.Extension.Equals(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                                f.Extension.Equals(".tga", System.StringComparison.OrdinalIgnoreCase) ||
                                f.Extension.Equals(".psd", System.StringComparison.OrdinalIgnoreCase) ||
                                f.Extension.Equals(".psb", System.StringComparison.OrdinalIgnoreCase));
                
                foreach (var file in allFiles)
                {
                    checkedCount++; // 每找到一个符合条件的文件就+1
                    string assetPath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                    if (textureImporter != null)
                    {
                        if (ApplySettings(textureImporter))
                        {
                            // 只有在设置被实际更改时才标记和计数
                            EditorUtility.SetDirty(textureImporter);
                            Debug.Log($"[更新] {assetPath} 的设置已在内存中更新。");
                            processedCount++;
                        }
                    }
                }
                // 处理 SpriteAtlas
                var atlasFiles = direction.GetFiles("*.spriteatlas", SearchOption.AllDirectories);
                foreach (var file in atlasFiles)
                {
                    atlasCheckedCount++;
                    string assetPath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                    if (ProcessSpriteAtlas(assetPath))
                    {
                        atlasProcessedCount++;
                    }
                }
            }
            
            // 提供更详细的最终日志
            Debug.Log($"--- 批量处理报告 ---");
            Debug.Log($"检查了 {checkedCount} 个纹理，更新了 {processedCount} 个。");
            Debug.Log($"检查了 {atlasCheckedCount} 个图集，更新了 {atlasProcessedCount} 个。");
            if (processedCount > 0)
            {
                Debug.Log("处理完成，正在保存并刷新资源数据库...");
            }
            else
            {
                Debug.Log("所有资源均已符合要求，无需修改。");
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            // 只有在真正有修改时才需要强制刷新
            if (processedCount > 0)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Debug.Log("所有被修改的纹理已重新导入！");
            }
        }
    }
    public static void ApplySettingsAndReimportForPostprocessor(string path, TextureImporter textureImporter)
    {
        if (ApplySettings(textureImporter))
        {
            // 在OnPreprocessTexture中，不需要调用SaveAndReimport，因为Unity会自动处理。
            // 但如果这个方法要在其他地方被独立调用，保留它也无妨，
            // 只是在预处理流程中它是多余的。为清晰起见，我们可以让Postprocessor直接调用ApplySettings。
            Debug.Log($"{path} 格式已在导入时自动更新。");
        }
    }
    private static bool ApplySettings(TextureImporter textureImporter)
    {
        if (textureImporter == null) return false;

        bool needsReimport = false;
        
        // 关闭mipmap开关
        if (textureImporter.mipmapEnabled)
        {
            textureImporter.mipmapEnabled = false;
            needsReimport = true;
        }
        
            if (!textureImporter.sRGBTexture)
            {
                textureImporter.sRGBTexture = true;
                needsReimport = true;
            }
        
        return needsReimport;
    }
    /// <summary>
    /// 处理单个 SpriteAtlas 资源
    /// </summary>
    /// <returns>如果设置被修改则返回 true</returns>
    private static bool ProcessSpriteAtlas(string assetPath)
    {
        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
        if (atlas == null) return false;

        bool changed = false;
        SpriteAtlasTextureSettings textureSettings = atlas.GetTextureSettings();
        // 关闭 Mipmap
        if (textureSettings.generateMipMaps == true || !textureSettings.sRGB)
        {
            textureSettings.generateMipMaps = false;
            textureSettings.sRGB = true;
            changed = true;
        }
        if (changed)
        {
            EditorUtility.SetDirty(atlas);
            Debug.Log($"[图集更新] {assetPath} 的设置已在内存中更新。");
        }
        return changed;
    }
}