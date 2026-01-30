using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using EscapeGame.Building.Config;
using EscapeGame.Building.Utils;
using LornaGame.Editor;
using UnityEditor;
using UnityEngine;
using LornaGame.ModuleExtensions;

namespace EscapeGame.Building{
    public class ShellInterfaceWindow : EditorWindow{
    #region Const params

        private const string COLOR_RICH_FORMAT = "<color={1}>{0}</color>";

    #endregion

        [MenuItem("Tools/打包工具 %t")]
        public static void MenuEntrance(){ EditorWindow.GetWindow<ShellInterfaceWindow>(); }

        private CommandParameters          _config;
        private ShellInterface             _shellInterface;
        private Dictionary<string, string> _tempconfig;
        private GUIStyle                   _tittleStyle;
        private Vector2                    _scrollPosition;
        private bool                       _steamConfig;

        private void OnEnable(){
            minSize         = new Vector2(400, 400);
            _config         = CommandParameters.LoadFromFile();
            _shellInterface = new ShellInterface();
            _tempconfig     = new Dictionary<string, string>(_config);
            foreach (var cmd in _shellInterface.ShellCmds){
                var pkgStr = cmd.GetType().GetCustomAttribute<CommandPkgStrAttribute>();
                if (!_tempconfig.ContainsKey(pkgStr.FullCommand)){
                    _tempconfig.Add(pkgStr.FullCommand, GetCmdDefaultValue(cmd));
                }
            }

            foreach (var cmd in _shellInterface.PreBuildCmds){
                var pkgStr = cmd.GetType().GetCustomAttribute<CommandPkgStrAttribute>();
                if (!_tempconfig.ContainsKey(pkgStr.FullCommand)){
                    _tempconfig.Add(pkgStr.FullCommand, GetCmdDefaultValue(cmd));
                }
            }

            if (null == _tittleStyle){
                _tittleStyle          = new GUIStyle{ fontStyle = FontStyle.Bold, fontSize = 15 };
                _tittleStyle.richText = true;
            }

            base.titleContent = new GUIContent{ text = "打包工具" };
        }

        public void OnGUI(){
            if (EditorApplication.isCompiling){
                return;
            }

            _scrollPosition = EditorHelper.ScrollView(_scrollPosition, ScrollContent);
        }

    #region Static config

        private void ScrollContent(){
            DrawStaticConfig();
            DrawPreCmd();
            DrawPkgCmd();
            DrawCmdCopy();
            DrawSequenceExecute();
        }

        private void DrawStaticConfig(){
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent(string.Format(COLOR_RICH_FORMAT, "静态配置", "cyan")), _tittleStyle);
            EditorHelper.Vertical(DrawStaticGroup, GUI.skin.box);
            EditorGUILayout.Space();
        }

        private void DrawStaticGroup(){
            //目录
            EditorHelper.Horizontal(DrawPkgRelative, GUI.skin.box);
            //自动压缩
            EditorHelper.Horizontal(DrawAutoPkgZip, GUI.skin.box);
            //资源版本号
            EditorHelper.Horizontal(DrawResVersion, GUI.skin.box);
            //Steam上传相关配置
            EditorHelper.Vertical(DrawSteamConfig, GUI.skin.box);
        }

        private void DrawSteamConfig(){
            _steamConfig = EditorGUILayout.BeginToggleGroup("Steam配置", _steamConfig);
            var cnf = BuildingCmdEditorConfig.Ins;
            cnf.SteamAppId         = EditorGUILayout.DelayedTextField("Steam[App]Id", cnf.SteamAppId);
            cnf.SteamDepotId       = EditorGUILayout.DelayedTextField("Steam[Depot]Id", cnf.SteamDepotId);
            cnf.SteamDesc          = EditorGUILayout.DelayedTextField("本地上传描述", cnf.SteamDesc);
            cnf.SteamVerbose       = EditorGUILayout.DelayedTextField("Verbose(当作数字版本号即可,每次新增同一个Depot时++)", cnf.SteamVerbose);
            cnf.SteamSetLive       = EditorGUILayout.DelayedTextField("SetLive(分支:默认default)", cnf.SteamSetLive);
            cnf.UploadSteamAccount = EditorGUILayout.DelayedTextField("Steam账号", cnf.UploadSteamAccount);
            cnf.UploadSteamPwd     = EditorGUILayout.DelayedTextField("Steam密码", cnf.UploadSteamPwd);
            if (GUILayout.Button("保存")){
                BuildingCmdEditorConfig.SaveImmediately();
            }

            EditorGUILayout.EndToggleGroup();
        }

        private static void DrawPkgRelative(){
            GUILayout.Label("出包目录(项目根目录且平台带有平台后缀):");
            DrawDirectFolder();
            BuildingCmdEditorConfig.Ins.OutputPkgPath = EditorGUILayout.DelayedTextField(BuildingCmdEditorConfig.Ins.OutputPkgPath);
            if (GUILayout.Button("打开目录")){
                string path = Path.Combine(Application.dataPath, BuildingCmdEditorConfig.Ins.OutputPkgPath);
                EditorUtility.RevealInFinder(path);
            }
        }

        private static void DrawResVersion(){
            GUILayout.Label("资源版本(自动=出包时间):");
            GUILayout.Label("Auto");
            BuildingCmdEditorConfig.Ins.AutoABResVersion = EditorGUILayout.Toggle(BuildingCmdEditorConfig.Ins.AutoABResVersion, GUILayout.Width(30));
            if (BuildingCmdEditorConfig.Ins.AutoABResVersion){
                string hookVer = BuildUtils.GetPackageVersion();
                if (!BuildingCmdEditorConfig.Ins.ABResVersion.Equals(hookVer)){
                    RefreshSaveAutoResVer(hookVer);
                }

                EditorGUILayout.LabelField(BuildingCmdEditorConfig.Ins.ABResVersion, GUILayout.Width(120));
            }
            else{
                string oldVer  = BuildingCmdEditorConfig.Ins.ABResVersion;
                string replace = EditorGUILayout.DelayedTextField(oldVer, GUILayout.Width(120));
                if (!oldVer.Equals(replace)){
                    RefreshSaveAutoResVer(replace);
                }
            }

            if (GUILayout.Button("刷新版本")){
                RefreshSaveAutoResVer(BuildingCmdEditorConfig.Ins.ABResVersion);
            }
        }

        private static void DrawDirectFolder(){
            GUILayout.Label("完成后自动定位文件夹:");
            bool tog = EditorGUILayout.Toggle(BuildingCmdEditorConfig.Ins.PackingEndDirectFolder, GUILayout.Width(30));
            if (tog != BuildingCmdEditorConfig.Ins.PackingEndDirectFolder){
                BuildingCmdEditorConfig.Ins.PackingEndDirectFolder = tog;
                BuildingCmdEditorConfig.SaveImmediately();
            }
        }

        private static void DrawAutoPkgZip(){
            GUILayout.Label("完成后自动压缩(自动区分backup文件与main文件)(比较耗时,赶时间不建议)");
			var nameList = System.Enum.GetNames(typeof(ArchivePkgType));
			List<ArchivePkgType> arr = new ();
			nameList.Ergodic(t=>{arr.Add(t.ToEnum<ArchivePkgType>());});

			EditorHelper.DropDown(BuildingCmdEditorConfig.Ins.AutoArchive, arr, (select) => {
				BuildingCmdEditorConfig.Ins.AutoArchive = select;
				BuildingCmdEditorConfig.SaveImmediately();
			});
            // bool tog = EditorGUILayout.Toggle(BuildingCmdEditorConfig.Ins.AutoArchive, GUILayout.Width(30));
            // if (tog != BuildingCmdEditorConfig.Ins.AutoArchive){
            //     BuildingCmdEditorConfig.Ins.AutoArchive = tog;
            //     BuildingCmdEditorConfig.SaveImmediately();
            // }
        }

        /// <summary>
        /// 刷新并保存自动资源版本
        /// </summary>
        private static void RefreshSaveAutoResVer(string newVersion){
            //主动修改Editor的资源版本,正式执行Build时,才去修改GameConfig的RuntimeABResVersion版本
            BuildingCmdEditorConfig.Ins.ABResVersion = newVersion;
            BuildingCmdEditorConfig.SaveImmediately();
            // var cnf = GameConfig.Instance;
            // cnf.ResVer = newVersion;
            // EditorUtility.SetDirty(cnf);
            // AssetDatabase.SaveAssetIfDirty(cnf);
            // AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

    #endregion

        private void DrawPreCmd(){
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent(string.Format(COLOR_RICH_FORMAT, " 预处理(Editor下手动)", "cyan")), _tittleStyle);
            EditorGUILayout.Space();
            foreach (var cmd in _shellInterface.PreBuildCmds){
                ShowCommand(cmd);
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
        }

        private void DrawPkgCmd(){
            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent(string.Format(COLOR_RICH_FORMAT, " 打包", "cyan")), _tittleStyle);
            EditorGUILayout.Space();
            foreach (var cmd in _shellInterface.ShellCmds){
                ShowCommand(cmd);
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
        }

        private void DrawCmdCopy(){
            EditorGUILayout.BeginHorizontal();
            var configstr = _config.ToString();
            EditorGUILayout.LabelField(configstr);
            if (GUILayout.Button("Copy", GUILayout.Height(20), GUILayout.Width(48))){
                EditorGUIUtility.systemCopyBuffer = configstr;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSequenceExecute(){
            EditorGUILayout.Space();
            if (GUILayout.Button("顺序执行", GUILayout.Height(32))){
                _config.SaveToFile();
                var si = new ShellInterface();
                _ = si.Run(_config);
            }

            EditorGUILayout.Space();
        }

        private string GetCmdDefaultValue(IBuildingCommand cmd){
            switch (cmd.CmdValType){
                case BuildingCmdValueType.None:
                case BuildingCmdValueType.StringVal:
                    return "";
                case BuildingCmdValueType.IntVal:  return "0";
                case BuildingCmdValueType.Choices: return cmd.Choices.FirstKey();
            }

            return "";
        }

        private string GetCmdHelpString(IBuildingCommand cmd){
            if (cmd.Help == null){
                return null;
            }

            if (cmd.CmdValType != BuildingCmdValueType.Choices){
                return cmd.Help;
            }
            else{
                StringBuilder sb = new StringBuilder(512);
                sb.Append(cmd.Help);
                foreach (var kv in cmd.Choices){
                    sb.Append("\n    ");
                    sb.Append(kv.Key);
                    sb.Append(" : ");
                    sb.Append(kv.Value);
                }

                return sb.ToString();
            }
        }

        private void ShowCommand(IBuildingCommand cmd){
            EditorGUILayout.BeginHorizontal();
            var    pkgStr = cmd.GetType().GetCustomAttribute<CommandPkgStrAttribute>();
            bool   isOn   = _config.ContainsKey(pkgStr.FullCommand);
            var    val    = _tempconfig[pkgStr.FullCommand];
            string cmdstr = pkgStr.ShortCommand == null ? "--" + pkgStr.FullCommand : "-" + pkgStr.ShortCommand;
            if (isOn){ _config.TryAdd(pkgStr.FullCommand, val); }

            if (isOn != EditorGUILayout.Toggle(isOn, GUILayout.Width(18))){
                if (isOn){
                    _config.Remove(pkgStr.FullCommand);
                }
                else{
                    _config.Add(pkgStr.FullCommand, val);
                }
            }

            if (GUILayout.Button(cmd.Name, GUILayout.Height(18), GUILayout.Width(90))){
                cmd.OnExecute(val, null);
            }

            switch (cmd.CmdValType){
                case BuildingCmdValueType.Choices:   ShowCmdValType_Choices(cmd, val, pkgStr, cmdstr); break;
                case BuildingCmdValueType.IntVal:    ShowCmdValType_IntVal(val, pkgStr); break;
                case BuildingCmdValueType.StringVal: ShowCmdValType_StringVal(val, pkgStr); break;
            }

            if (null != cmd.OnEditorExtra){
                if (GUILayout.Button("⊙", GUILayout.Height(14), GUILayout.Width(21))){
                    cmd.OnEditorExtra(val);
                }
            }

            if (null != cmd.Help){
                if (GUILayout.Button("?", GUILayout.Height(14), GUILayout.Width(21))){
                    EditorUtility.DisplayDialog(cmd.Name, GetCmdHelpString(cmd), "好的");
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        //TODO:可优化
        private void ShowCmdValType_Choices(IBuildingCommand cmd, string val, CommandPkgStrAttribute pkgStr, string cmdstr){
		    if (string.IsNullOrEmpty(val)) { return; }

		    if (EditorGUILayout.DropdownButton(new GUIContent{ text = $"{cmd.Choices[val]}  ( {cmdstr} {val} )" }, FocusType.Passive)){
                GenericMenu menu = new GenericMenu();
                foreach (var kv in cmd.Choices){
                    string newstr = string.Copy(kv.Key);
                    menu.AddItem(
                                 new GUIContent{ text = cmd.Choices[newstr] },
                                 newstr == val,
                                 () => {
                                     _tempconfig[pkgStr.FullCommand] = newstr;
                                     if (_config.ContainsKey(pkgStr.FullCommand)){
                                         _config[pkgStr.FullCommand] = newstr;
                                     }
                                 }
                                );
                }

                menu.ShowAsContext();
            }
        }

        //TODO:可优化
        private void ShowCmdValType_IntVal(string val, CommandPkgStrAttribute pkgStr){
            int v = EditorGUILayout.IntField(int.Parse(val));
            _tempconfig[pkgStr.FullCommand] = v.ToString();
            if (_config.ContainsKey(pkgStr.FullCommand)){
                _config[pkgStr.FullCommand] = v.ToString();
            }
        }

        //TODO:可优化
        private void ShowCmdValType_StringVal(string val, CommandPkgStrAttribute pkgStr){
            val                             = EditorGUILayout.TextField(val);
            _tempconfig[pkgStr.FullCommand] = val;
            if (_config.ContainsKey(pkgStr.FullCommand)){
                _config[pkgStr.FullCommand] = val;
            }
        }
    }
}
