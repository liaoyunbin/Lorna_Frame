using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EscapeGame.Building.Config;
using EscapeGame.Utils;
using UnityEditor;
using UnityEngine;

namespace EscapeGame.Building{
    /// <summary>
    /// 当前仅先支持上传到Steam后台
    /// </summary>
    [CommandPkgStr("steam", "upload-steam")]
    public class BuildingCmdUploadAppSteam : IBuildingCommand{
    #region Const

        //SteamSDK上传工具工作目录
        private const string BUILDER_DIR = "SteamSdk\\tools\\ContentBuilder";

        //vdf 模板配置路径
        private const string BUILD_VDF_SAVE_PATH = "scripts\\app_build.vdf";

        //Steam.exe 目录
        private const string STEAM_CMD_PATH = "builder\\steamcmd.exe";

    #endregion

        public string               Name       => "上传Steam后台";
        public string               Help       => "repack Steam需求的资源后上传的到Steam服务器";
        public BuildingCmdValueType CmdValType => BuildingCmdValueType.Choices;

        // public Dictionary<string, string> Choices{ get; } = new (){ { "pkg", "仅上传整包" },{ "ab", "仅上传资源包" },{ "all", "上传整包+资源包" }, };

        public Action<string>             OnEditorExtra => null;

		public Dictionary<string, string> Choices { get; } = new() { { "default", "上传包" }, { "DRM", "将drm包装过的包 放到 WindowsDRM 目录，并上传DRM exe包" }, };
		#if UNITY_STANDALONE_LINUX
        private string inputFolder => Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}Linux"); // 源文件夹路径
        private string outputFolder => Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}LinuxDRM"); // 目标文件夹路径

        private string inputFilename => inputFolder + $"/{GameConfig.Instance.AppShortName}.bin"; // 源文件夹exe
        private string outputFileName => outputFolder + $"/{GameConfig.Instance.AppShortName}.bin";  // 目标文件夹exe 
		#elif UNITY_STANDALONE_WIN
		private string inputFolder => Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}Windows"); // 源文件夹路径
        private string outputFolder => Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}WindowsDRM"); // 目标文件夹路径

        private string inputFilename => inputFolder + $"/{GameConfig.Instance.AppShortName}.exe"; // 源文件夹exe
        private string outputFileName => outputFolder + $"/{GameConfig.Instance.AppShortName}.exe";  // 目标文件夹exe 
		#endif

        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output)
        {
            bool hasDRM = (arg == "DRM");
            if (hasDRM)
            {
                bool flag = SyncAndCleanup(inputFolder, outputFolder, outputFileName);
                if (!flag)
                {
                    return false;
                }
            }
            return await DoUploadAppSteam(hasDRM);
        }

        public async Task<bool> DoUploadAppSteam(bool hasDRM)
        {
            //手动逐层获取Director,不加..\\防止其他维护人看不明白
            string parentPath   = Path.GetDirectoryName(Application.dataPath);
            string projectPath  = Path.GetDirectoryName(parentPath);
            string steamRoot    = $"{projectPath}\\{BUILDER_DIR}"; //SteamSDK上传工具工作目录
            string vdfSavePath  = $"{steamRoot}\\{BUILD_VDF_SAVE_PATH}";
            string steamCmdPath = $"{steamRoot}\\{STEAM_CMD_PATH}";

            //config 
            BuildingCmdEditorConfig cnf = BuildingCmdEditorConfig.Ins;

            //reset config
            //当前项目只有windows对应steam平台,写死即可

            string uploadOutputLogPath = Path.Combine(Application.dataPath, $"{BuildingCmdEditorConfig.Ins.OutputPkgPath}SteamOutput\\");
            if (!Directory.Exists(uploadOutputLogPath)){
                Directory.CreateDirectory(uploadOutputLogPath);
            }
            string buildPath = hasDRM ? outputFolder + $"\\" : inputFolder + $"\\";
            string resetConfig = SetConfig(
                                           cnf.SteamAppId,
                                           cnf.SteamDesc,
                                           cnf.SteamVerbose,
                                           cnf.SteamSetLive,
                                           buildPath,
                                           uploadOutputLogPath,
                                           cnf.SteamDepotId
                                          );
            File.WriteAllText(vdfSavePath, resetConfig);
            //build arg
            string steamUploadArg = !hasDRM ?
                BuildArg(cnf.UploadSteamAccount, cnf.UploadSteamPwd, vdfSavePath) :
                BuildDRMArg(cnf.UploadSteamAccount, cnf.UploadSteamPwd, vdfSavePath, cnf.SteamAppId, inputFilename, outputFileName);


            steamUploadArg = $"{steamCmdPath} {steamUploadArg}";
            //Unity 直接开process走cmd有局限性,不好动态输入cmd新arg,因为可能存在需要令牌输入。
            string batScriptName = steamRoot + "\\" + "run_steam_upload.bat";
            File.WriteAllText(batScriptName, steamUploadArg);

            UnityEngine.Debug.Log(steamUploadArg);
            //run cmd
            // var    process       = Run(steamCmdPath, steamUploadArg, ".", true);
            var process = Run(batScriptName, string.Empty, ".", false);

            //监听输出
            process.OutputDataReceived += (s, ev) => { UnityEngine.Debug.Log(ev.Data); };
            process.ErrorDataReceived  += (s, ev) => { UnityEngine.Debug.Log(ev.Data); };
            process.Exited             += (s, ev) => { UnityEngine.Debug.Log(ev.ToString()); };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            string processOutput = process.StandardOutput.ReadToEnd();
            if (process.ExitCode != 0){
                string errorLog = $"[SteamCmd] 上传异常!!输出信息:\\r\\n {processOutput}, \\r\\n 请检查上传参数、配置、以及content目录!!";
                EditorUtility.DisplayDialog("Caution", errorLog, "OK");
                return false;
            }
            else{
                EditorUtility.DisplayDialog("成功!", "包体上传完成!请检查Steam后台。", "OK");
            }

            return true;
        }

    #region Private utils functions

        private string BuildArg(string account, string pwd, string vdfPath)
        {
            string rel = $"+login {account} {pwd} +run_app_build {vdfPath} +quit";
            return rel;
        }
        private string BuildDRMArg(string account, string pwd, string vdfPath, string appid, string inputFilename, string outputFilename)
        {
            string drm = $"+drm_wrap {appid} {inputFilename} {outputFilename} drmtoolp 0";
            string rel = $"+login {account} {pwd} {drm} +run_app_build {vdfPath} +quit";
            return rel;
        }

        private static Process Run(string exe, string arguments, string working_dir = ".", bool wait_exit = false){
            try{
                ProcessStartInfo info = new ProcessStartInfo{
                                                                FileName               = exe,
                                                                Arguments              = arguments,
                                                                CreateNoWindow         = false,
                                                                UseShellExecute        = false,
                                                                WorkingDirectory       = working_dir,
                                                                RedirectStandardOutput = true,
                                                                RedirectStandardError  = true,
                                                                RedirectStandardInput  = true,
                                                                StandardOutputEncoding = Encoding.GetEncoding("utf-8"),
                                                            };
                Process process = Process.Start(info);
                if (wait_exit){
                    WaitForExitAsync(process).ConfigureAwait(false);
                }

                return process;
            }
            catch (Exception e){
                throw new Exception($"Run process exception:  command: {exe} {arguments}", e);
            }
        }

        private static async Task WaitForExitAsync(Process process){
            if (!process.HasExited){
                return;
            }

            try{
                process.EnableRaisingEvents = true;
            }
            catch (InvalidOperationException){
                if (process.HasExited){
                    return;
                }

                throw;
            }

            var tcs = new TaskCompletionSource<bool>();
            void Handler(object s, EventArgs e) => tcs.TrySetResult(true);
            process.Exited += Handler;
            try{
                if (process.HasExited){
                    return;
                }

                await tcs.Task;
            }
            finally{
                process.Exited -= Handler;
            }
        }

        private string SetConfig(string appId, string desc, string verbose, string setLive, string contentRoot, string outputRoot, string depotId){
            return $@"
""AppBuild""
{{
	""AppID"" ""{appId}""
	""Desc"" ""{desc}""
	""verbose"" ""{verbose}"" 
	""SetLive"" ""{setLive}""

	""ContentRoot"" ""{contentRoot}"" 
	""BuildOutput"" ""{outputRoot}"" 

	""Depots""
	{{
		""{depotId}""
		{{
			""FileMapping""
			{{
				""LocalPath"" ""*"" // all files from contentroot folder
				""DepotPath"" ""."" // mapped into the root of the depot
				""recursive"" ""1"" // include all subfolders
			}}
		}}
	}}
}}
";
        }


        private bool SyncAndCleanup(string folderPathA, string folderPathB, string fileToDelete)
        {
            // 1. 删除文件夹B（如果存在）
            if (Directory.Exists(folderPathB))
            {
                try
                {
                    Directory.Delete(folderPathB, true); // true表示递归删除所有子内容和文件
                    UnityEngine.Debug.Log($"已删除文件夹: {folderPathB}");
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError($"删除文件夹B失败: {e.Message}");
                    return false;
                }
            }

            // 2. 创建文件夹B
            try
            {
                Directory.CreateDirectory(folderPathB);
                UnityEngine.Debug.Log($"已创建文件夹: {folderPathB}");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"创建文件夹B失败: {e.Message}");
                return false;
            }

            // 3. 检查文件夹A是否存在，并复制其全部内容到B
            if (!Directory.Exists(folderPathA))
            {
                UnityEngine.Debug.LogError($"源文件夹A不存在: {folderPathA}");
                return false;
            }

            try
            {
                CopyDirectory(folderPathA, folderPathB);
                UnityEngine.Debug.Log($"已将文件夹A内容复制到文件夹B");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"复制文件夹A内容失败: {e.Message}");
                return false;
            }

            // 4. 删除B文件夹中的指定文件
            string targetFilePath = Path.Combine(folderPathB, fileToDelete);
            if (File.Exists(targetFilePath))
            {
                try
                {
                    File.Delete(targetFilePath);
                    UnityEngine.Debug.Log($"已删除指定文件: {targetFilePath}");
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogWarning($"删除指定文件失败: {e.Message}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 递归复制整个文件夹（包括所有子文件夹和文件）
        /// </summary>
        /// <param name="sourcePath">源文件夹路径</param>
        /// <param name="destinationPath">目标文件夹路径</param>
        private void CopyDirectory(string sourcePath, string destinationPath)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
            DirectoryInfo targetDir = new DirectoryInfo(destinationPath);

            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException($"源文件夹不存在: {sourcePath}");
            }

            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            // 复制所有文件
            foreach (FileInfo file in sourceDir.GetFiles())
            {
                string destFilePath = Path.Combine(targetDir.FullName, file.Name);
                file.CopyTo(destFilePath, true); // true表示覆盖已存在的文件
            }

            // 递归复制所有子文件夹
            foreach (DirectoryInfo subDir in sourceDir.GetDirectories())
            {
                string destSubPath = Path.Combine(targetDir.FullName, subDir.Name);
                CopyDirectory(subDir.FullName, destSubPath);
            }
        }
        #endregion
    }
}
