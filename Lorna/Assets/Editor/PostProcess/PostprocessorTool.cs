using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PostprocessorTool : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        var settings = TextureProcessorSettings.Instance;
        // 如果没有找到配置文件，或者配置文件里没有路径，则不执行任何操作
        if (settings == null || settings.targetFolders.Count == 0)
        {
            return;
        }

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        
        foreach (var folderPath in settings.targetFolders)
        {
            if (string.IsNullOrEmpty(folderPath)) continue;

            // 避免子文件夹名碰巧包含父文件夹名的问题
            if (assetPath.StartsWith(folderPath))
            {
                // 如果路径匹配，则应用设置并停止检查其他路径
                TextureProcessorLogic.ApplySettingsAndReimportForPostprocessor(assetPath, textureImporter);
                return; 
            }
        }
    }
    // private const string TextureFolderPath = "Assets/Resources"; // UGUI 文件路径
    // void OnPreprocessTexture()
    // {
    //     //自动设置类型;
    //     TextureImporter textureImporter = (TextureImporter)assetImporter;
    //     var path = AssetDatabase.GetAssetPath(textureImporter);
    //     //主要是针对是否导入项目某个文件夹下进行判断
    //     if (path.Contains(TextureFolderPath))
    //     {
    //         SetTexture(path, textureImporter);
    //     }    
    //}

    //[MenuItem("Tools/Resources目录下纹理工具设置【开启纹理的SRGB.关闭纹理的mipmap】")]
    // public static void SetAllTextureType()
    // {
    //     SetPathAllTexture(TextureFolderPath);
    //     AssetDatabase.ImportAsset(TextureFolderPath, ImportAssetOptions.ForceUpdate);
    // }
    //
    // private static void SetPathAllTexture(string _path)
    // {
    //     //针对目录下的所有文件进行遍历 取出.png和.jpg文件进行处理 可自行拓展
    //     DirectoryInfo direction = new DirectoryInfo(_path);
    //     FileInfo[] pngFiles = direction.GetFiles("*.png", SearchOption.AllDirectories);
    //     FileInfo[] jpgfiles = direction.GetFiles("*.jpg", SearchOption.AllDirectories);
    //     SetAllFilesTexture(pngFiles);
    //     SetAllFilesTexture(jpgfiles);
    // }
    //
    // private static void SetAllFilesTexture(FileInfo[] fileInfo)
    // {
    //     for (int i = 0; i < fileInfo.Length; i++)
    //     {
    //         //这里第一次写时有一个接口可直接调用，但是第二次写时找不到了 所以用了切割字符
    //         string str = fileInfo[i].FullName;
    //         string resultA = str.Replace("Assets", "|");
    //         string[] splitStr = resultA.Split('|');
    //         string path = "Assets" + splitStr[1];
    //         TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
    //         SetTexture(path, textureImporter);
    //     }
    // }
    //
    // /// <summary>
    // /// 开启纹理的SRGB.关闭纹理的mipmap
    // /// </summary>
    // /// <param name="_path"></param>
    // /// <param name="textureImporter"></param>
    // private static void SetTexture(string _path, TextureImporter textureImporter)
    // {
    //     string processInformation = "";
    //     if (textureImporter != null)
    //     {
    //         //开启SRGB
    //         if (!textureImporter.sRGBTexture)
    //         {
    //             textureImporter.sRGBTexture = true;
    //             processInformation += " 开启了SRGB";
    //         }
    //
    //         //关闭mipmap开关
    //         if (textureImporter.mipmapEnabled)
    //         {
    //             textureImporter.mipmapEnabled = false;
    //             processInformation += " 关闭mipmap开关";
    //         }
    //
    //         Debug.Log($"{_path} 格式化： {processInformation}");
    //     }
    // }
}


