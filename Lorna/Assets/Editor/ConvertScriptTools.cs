
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork_UI
{
    public class ConvertScriptTools : UnityEditor.Editor
    {
        [MenuItem("Assets/文件转换/选中文件 Convert Scripts to UTF-8", false, 110)]
        private static void AConvertScriptsToUTF8()
        {
            string[] guids = Selection.assetGUIDs;//获取当前选中的asset的GUID
            for (int i = 0; i < guids.Length; i++)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[i]);//通过GUID获取路径
                string scriptContent = File.ReadAllText(scriptPath, Encoding.GetEncoding("GB2312"));
                File.WriteAllText(scriptPath, scriptContent, Encoding.UTF8);
            }
            //Logs.Log("Scripts converted to UTF-8 encoding.");
        }


        [MenuItem("Assets/文件转换/检测项目中可能是GB2312-8格式 的所有c#代码", false, 109)]
        private static void ScanProject()
        {
            // 获取项目中所有的.cs文件
            string[] allCsFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            int gb2312Count = 0;

            try
            {
                for (int i = 0; i < allCsFiles.Length; i++)
                {
                    string filePath = allCsFiles[i];
                    // 更新进度条
                    if (EditorUtility.DisplayCancelableProgressBar("扫描文件中", $"正在检测: {Path.GetFileName(filePath)}", (float)i / allCsFiles.Length))
                    {
                        break;
                    }

                    System.Text.Encoding detectedEncoding = DetectFileEncoding(filePath);
                    string relativePath = filePath.Replace(Application.dataPath, "Assets");
                    string result = $"{relativePath} : {detectedEncoding.EncodingName}";

                    if (detectedEncoding.CodePage == 936) // GB2312的代码页是936
                    {
                        result += " (可能需要转换)";
                        UnityEngine.Debug.Log(result);
                        gb2312Count++;
                    }
                }

                EditorUtility.ClearProgressBar();
                // 显示统计结果
                string summary = $"扫描完成！共扫描{allCsFiles.Length}个文件，其中{gb2312Count}个可能是GB2312编码。";
                UnityEngine.Debug.Log(summary);
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                UnityEngine.Debug.LogError($"扫描过程中发生错误: {e.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 检测文件的编码格式
        /// </summary>
        private static System.Text.Encoding DetectFileEncoding(string filePath)
        {
            byte[] buffer = new byte[4]; // 读取文件的前几个字节用于分析
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(buffer, 0, 4);
            }

            // 1. 首先检查BOM（字节顺序标记）
            if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            {
                return Encoding.UTF8; // 带BOM的UTF-8
            }

            // 2. 如果没有BOM，则分析文件内容来判断编码
            byte[] fileContent = File.ReadAllBytes(filePath);
            if (IsUTF8Bytes(fileContent))
            {
                return Encoding.UTF8; // 不带BOM的UTF-8
            }

            // 3. 在中文Windows环境下，默认编码往往是GB2312
            // 注意：这只是推测，因为很多ANSI编码在中文环境下就是GB2312
            return Encoding.GetEncoding("GB2312"); // 返回系统默认的ANSI编码（通常是GB2312）
        }

        /// <summary>
        /// 判断字节流是否符合UTF-8编码规则（即使没有BOM）
        /// 此方法由社区开发者共享，用于检测不带BOM的UTF-8文件
        /// </summary>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; // 计算当前正分析的字符应还有的字节数
            byte curByte;

            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80) // 最高位为1，表示可能是多字节字符
                    {
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        // UTF-8字符长度在1到6字节之间，但现代UTF-8通常最多4字节
                        if (charByteCounter == 1 || charByteCounter > 4)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    // 后续字节必须以10开头
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            return charByteCounter == 1;
        }
    }
}
