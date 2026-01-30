using UnityEditor;
using System.Diagnostics;
using UnityEngine;

public static class LubanGenerator
{
    [MenuItem("Tools/Luban/Generate Configs")] // 在Unity顶部菜单栏创建选项
    public static void Generate()
    {
        // 1. 定义路径（请根据你的项目结构修改这些路径）
        string batFileName = "gen.bat";
        string workingDirectory = Application.dataPath + "/../lubanConfig"; // 指向你的gen.bat所在目录

        // 2. 设置进程启动信息
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = batFileName;
        startInfo.WorkingDirectory = workingDirectory; // 设置工作目录，确保bat内的相对路径正确
        startInfo.UseShellExecute = true; // 使用操作系统shell启动进程
        startInfo.CreateNoWindow = false;  // 是否显示命令行窗口

        // 3. 启动进程
        try
        {
            Process process = Process.Start(startInfo);
            process.WaitForExit(); // 等待bat文件执行完毕

            if (process.ExitCode == 0)
            {
                UnityEngine.Debug.Log("Luban配置生成成功！");
                AssetDatabase.Refresh(); // 刷新Unity资源数据库，让新生成的文件立即可见
            }
            else
            {
                UnityEngine.Debug.LogError($"Luban配置生成失败，退出码: {process.ExitCode}");
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"调用gen.bat时出错: {e.Message}");
        }
    }
}