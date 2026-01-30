//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using EscapeGame.Building.Config;
//using EscapeGame.Utils;
//using UnityEditor;

//namespace EscapeGame.Building{
//    /// <summary>
//    /// 当前资源量使然,仅考虑ScriptableBuildPipeline
//    /// </summary>
//    [CommandPkgStr(shortCommand : "ab", fullCommand : "abres")]
//    public class BuildingCmdPackABRes : IBuildingCommand{
//        private enum ABResType{
//            Full      = 0, //默认全量打包
//            Cache     = 1, //使用缓存全量打包(若有缓存的话)
//            First     = 2, //构建首包(默认清理全量打包)
//            FirstCopy = 3, //仅从缓存Copy首包的包
//        }

//        public string               Name       => "Build ABRes";
//        public string               Help       => "构建AssetBundle资产,当前资产走SBP";
//        public BuildingCmdValueType CmdValType => BuildingCmdValueType.Choices; //暂不提供额外选项

//        public Dictionary<string, string> Choices{ get; } =
//            new(){ { "full", "清理缓存,重新构建" },{ "cache", "不清理缓存,直接使用缓存构建(若有缓存的话)" },{ "first", "构建首包(默认清理缓存)" },{ "firstCopy", "仅从缓存Copy首包的包" } };

//        public  Action<string> OnEditorExtra => null;
//        private string         BUNDLE_NAME   => GameConfig.Instance.ResPack;
//        private string         PIPELINE_NAME => BuildingConst.PIPELINE_NAME;
//        private ABResType      _abResType = ABResType.Full; //默认全量打包

//    #region Private params

//        protected BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;

//    #endregion

//        public async Task<bool> OnExecute(string arg, Dictionary<string, string> output){
//            switch (arg){
//                case "full":      _abResType = ABResType.Full; break;
//                case "cache":     _abResType = ABResType.Cache; break;
//                case "first":     _abResType = ABResType.First; break;
//                case "firstCopy": _abResType = ABResType.FirstCopy; break;
//            }
//            //主动修改GameConfig的版本
//            AlterRuntimeABResVersion();
//            //
//            var fileNameStyle         = EFileNameStyle.HashName;
//            var buildinFileCopyOption = _abResType == ABResType.First ? EBuildinFileCopyOption.None : EBuildinFileCopyOption.ClearAndCopyAll;
//            var buildinFileCopyParams = AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(BUNDLE_NAME, PIPELINE_NAME);
//            var compressOption        = AssetBundleBuilderSetting.GetPackageCompressOption(BUNDLE_NAME, PIPELINE_NAME);
//            var clearBuildCache =
//                _abResType == ABResType.Full ||
//                _abResType == ABResType.First; //AssetBundleBuilderSetting.GetPackageClearBuildCache(BUNDLE_NAME, PIPELINE_NAME);
//            var                       useAssetDependencyDB    = AssetBundleBuilderSetting.GetPackageUseAssetDependencyDB(BUNDLE_NAME, PIPELINE_NAME);
//            var                       builtinShaderBundleName = GetBuiltinShaderBundleName();
//            ScriptableBuildParameters buildParameters         = new ScriptableBuildParameters();
//            buildParameters.BuildOutputRoot          = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
//            buildParameters.BuildinFileRoot          = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
//            buildParameters.BuildPipeline            = PIPELINE_NAME;
//            buildParameters.BuildBundleType          = (int)EBuildBundleType.AssetBundle;
//            buildParameters.BuildTarget              = BuildTarget;
//            buildParameters.PackageName              = BUNDLE_NAME;
//            buildParameters.PackageVersion           = GameConfig.Instance.ResVer;
//            buildParameters.EnableSharePackRule      = true;
//            buildParameters.VerifyBuildingResult     = true;
//            buildParameters.FileNameStyle            = fileNameStyle;
//            buildParameters.BuildinFileCopyOption    = buildinFileCopyOption;
//            buildParameters.BuildinFileCopyParams    = buildinFileCopyParams;
//            buildParameters.CompressOption           = compressOption;
//            buildParameters.ClearBuildCacheFiles     = clearBuildCache;
//            buildParameters.UseAssetDependencyDB     = useAssetDependencyDB;
//            buildParameters.BuiltinShadersBundleName = builtinShaderBundleName;
//            buildParameters.EncryptionServices       = CreateEncryptionServicesInstance();
//            buildParameters.ManifestProcessServices  = CreateManifestProcessServicesInstance();
//            buildParameters.ManifestRestoreServices  = CreateManifestRestoreServicesInstance();
//            ScriptableBuildPipeline pipeline    = new ScriptableBuildPipeline();
//            BuildResult             buildResult = null;
//            if (_abResType != ABResType.FirstCopy){
//                buildResult = pipeline.Run(buildParameters, true);
//            }
//            else{
//                buildResult = new BuildResult(){ Success = true };
//            }

//            if (buildResult.Success){
//                UnityEngine.Debug.Log($"[BuildingCmdPackABRes][资源构建]:成功!!!".GreenColor());
//            }
//            else{
//                UnityEngine.Debug.Log($"[BuildingCmdPackABRes][资源构建]:失败! {buildResult.ErrorInfo}".RedColor());
//            }

//            //首包逻辑我们自己拷贝即可
//            if (_abResType == ABResType.First || _abResType == ABResType.FirstCopy){
//                FirstBundlePack(buildParameters);
//            }

//            return buildResult.Success;
//        }

//        /// <summary>
//        /// 修改Runtime内的ABRes版本,给Runtime校验版本用
//        /// </summary>
//        private void AlterRuntimeABResVersion(){
//            //Hook Version
//            string newVersion = BuildingCmdEditorConfig.Ins.ABResVersion;
//            //
//            var cnf = GameConfig.Instance;
//            cnf.ResVer = newVersion;
//            EditorUtility.SetDirty(cnf);
//            AssetDatabase.SaveAssetIfDirty(cnf);
//            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
//        }

//        /// <summary>
//        /// 内置着色器资源包名称
//        /// 注意：和自动收集的着色器资源包名保持一致！
//        /// </summary>
//        protected string GetBuiltinShaderBundleName(){
//            var uniqueBundleName = AssetBundleCollectorSettingData.Setting.UniqueBundleName;
//            var packRuleResult   = DefaultPackRule.CreateShadersPackRuleResult();
//            return packRuleResult.GetBundleName(BUNDLE_NAME, uniqueBundleName);
//        }

//        /// <summary>
//        /// 创建资源加密服务类实例
//        /// </summary>
//        protected IEncryptionServices CreateEncryptionServicesInstance(){
//            var className  = AssetBundleBuilderSetting.GetPackageEncyptionServicesClassName(BUNDLE_NAME, PIPELINE_NAME);
//            var classTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
//            var classType  = classTypes.Find(x => x.FullName.Equals(className));
//            if (classType != null)
//                return (IEncryptionServices)Activator.CreateInstance(classType);
//            else
//                return null;
//        }

//        /// <summary>
//        /// 创建资源清单服务类实例
//        /// </summary>
//        /// <summary>
//        /// 创建资源清单加密服务类实例
//        /// </summary>
//        protected IManifestProcessServices CreateManifestProcessServicesInstance(){
//            var className  = AssetBundleBuilderSetting.GetPackageManifestProcessServicesClassName(BUNDLE_NAME, PIPELINE_NAME);
//            var classTypes = EditorTools.GetAssignableTypes(typeof(IManifestProcessServices));
//            var classType  = classTypes.Find(x => x.FullName.Equals(className));
//            if (classType != null)
//                return (IManifestProcessServices)Activator.CreateInstance(classType);
//            else
//                return null;
//        }

//        /// <summary>
//        /// 创建资源清单解密服务类实例
//        /// </summary>
//        protected IManifestRestoreServices CreateManifestRestoreServicesInstance(){
//            var className  = AssetBundleBuilderSetting.GetPackageManifestRestoreServicesClassName(BUNDLE_NAME, PIPELINE_NAME);
//            var classTypes = EditorTools.GetAssignableTypes(typeof(IManifestRestoreServices));
//            var classType  = classTypes.Find(x => x.FullName.Equals(className));
//            if (classType != null)
//                return (IManifestRestoreServices)Activator.CreateInstance(classType);
//            else
//                return null;
//        }

//    #region BunildPost ResBundle后置处理逻辑,当前主要用于首包处理

//        /// <summary>
//        /// 首包配置文件
//        /// </summary>
//        public const string F_BUNDLE_CONFIG = "Assets/Editor/MenuTools/Packing/Config/FirstPackingBundleConfig.json";

//        /// <summary>
//        /// 首包额外Pack配置文件
//        /// </summary>
//        public const string F_BUNDLE_EXTEND_PACK_CONFIG = "Assets/Editor/MenuTools/Packing/Config/FirstPackingExtendBundleConfig.json";

//        public const string F_RUNNING_FIRST = "Assets/Editor/MenuTools/Packing/Config/FirstPackingBundleState.json";

//        /// <summary>
//        /// 首包逻辑处理
//        /// </summary>
//        private void FirstBundlePack(ScriptableBuildParameters buildParameters){
//            string buildinRootDirectory   = buildParameters.GetBuildinRootDirectory();
//            string packageOutputDirectory = buildParameters.GetPackageOutputDirectory();
//            string buildPackageName       = buildParameters.PackageName;
//            string buildPackageVersion    = buildParameters.PackageVersion;
//            //首包默认就清空内置文件的目录
//            {
//                EditorTools.ClearFolder(buildinRootDirectory);
//            }

//            //拷贝补丁清单文件
//            {
//                string fileName   = YooAssetSettingsData.GetManifestBinaryFileName(buildPackageName, buildPackageVersion);
//                string sourcePath = $"{packageOutputDirectory}/{fileName}";
//                string destPath   = $"{buildinRootDirectory}/{fileName}";
//                if (File.Exists(sourcePath)){
//                    UnityEngine.Debug.Log($"拷贝补丁清单文件 : {sourcePath} -> {destPath}".GreenColor());
//                    EditorTools.CopyFile(sourcePath, destPath, true);
//                }
//            }

//            // 拷贝补丁清单哈希文件
//            {
//                string fileName   = YooAssetSettingsData.GetPackageHashFileName(buildPackageName, buildPackageVersion);
//                string sourcePath = $"{packageOutputDirectory}/{fileName}";
//                string destPath   = $"{buildinRootDirectory}/{fileName}";
//                if (File.Exists(sourcePath)){
//                    UnityEngine.Debug.Log($"拷贝补丁清单哈希文件 : {sourcePath} -> {destPath}".GreenColor());
//                    EditorTools.CopyFile(sourcePath, destPath, true);
//                }
//            }

//            // 拷贝补丁清单版本文件
//            {
//                string fileName   = YooAssetSettingsData.GetPackageVersionFileName(buildPackageName);
//                string sourcePath = $"{packageOutputDirectory}/{fileName}";
//                string destPath   = $"{buildinRootDirectory}/{fileName}";
//                if (File.Exists(sourcePath)){
//                    UnityEngine.Debug.Log($"拷贝补丁清单版本文件 : {sourcePath} -> {destPath}".GreenColor());
//                    EditorTools.CopyFile(sourcePath, destPath, true);
//                }
//            }

//            //加载跑出的首包json
//            string       firstBundleJson      = FileUtility.ReadAllText(F_BUNDLE_CONFIG);
//            List<string> buildinAssetPathList = JsonUtils.JsonToObject<List<string>>(firstBundleJson);
//            //加载已经构建的清单
//            string manifestName = YooAssetSettingsData.GetManifestJsonFileName(buildPackageName, buildPackageVersion);
//            // string                 manifestJson = FileUtility.ReadAllText(MANIFEST_CONFIG);
//            string          manifestPath = $"{packageOutputDirectory}/{manifestName}";
//            string          manifestJson = FileUtility.ReadAllText(manifestPath);
//            PackageManifest manifest     = ManifestTools.DeserializeFromJson(manifestJson);
//            //特殊处理,Dict为空时手动加
//            {
//                ManifestCheckReload(manifest);
//            }
//            //主动拉取首包的bundle以及对应依赖包
//            HashSet<PackageBundle> bundles = new HashSet<PackageBundle>();
//            foreach (var assetPath in buildinAssetPathList){
//                //包含后缀
//                if (manifest.TryGetPackageAsset(assetPath, out PackageAsset packageAsset)){
//                    var packageBundle = manifest.BundleList[packageAsset.BundleID];
//                    UnityEngine.Debug.Log($"[BuildingCmdPackABRes]获取包名:{packageBundle.BundleName}".YellowColor());
//                    if (bundles.Add(packageBundle)){
//                        int depth = 0;
//                        GetDependsPack(manifest, packageBundle, bundles, ref depth);
//                    }
//                }

//                //去除后缀(我们项目接入后期,为了不动旧逻辑的适配,有些包有后缀,有些包没有,所以保险期间都查一遍即可)
//                string cutSuffixPath = StringUtils.CutSuffix(assetPath);
//                if (manifest.TryGetPackageAsset(cutSuffixPath, out packageAsset)){
//                    var packageBundle = manifest.BundleList[packageAsset.BundleID];
//                    UnityEngine.Debug.Log($"[BuildingCmdPackABRes]获取包名:{packageBundle.BundleName}".YellowColor());
//                    if (bundles.Add(packageBundle)){
//                        int depth = 0;
//                        GetDependsPack(manifest, packageBundle, bundles, ref depth);
//                    }
//                }
//            }

//            //手动拷贝首包文件
//            string root = $"{AssetBundleBuilderHelper.GetStreamingAssetsRoot()}/{BUNDLE_NAME}";
//            foreach (var packageBundle in bundles){
//                string fileDetails = buildParameters.FileNameStyle == EFileNameStyle.HashName
//                    ? $"{packageBundle.FileHash}.bundle"
//                    : packageBundle.BundleName;
//                string sourcePath = $"{packageOutputDirectory}/{fileDetails}";
//                string destPath   = $"{root}/{fileDetails}";
//                if (!File.Exists(sourcePath)){
//                    UnityEngine.Debug.Log($"[BuildingCmdPackABRes]无效包路径(Asset direct path):{sourcePath}".RedColor());
//                    continue;
//                }

//                EditorTools.CopyFile(sourcePath, destPath, true);
//            }

//            string       firstExtendBundleJson      = FileUtility.ReadAllText(F_BUNDLE_EXTEND_PACK_CONFIG);
//            List<string> buildinExtendAssetPathList = JsonUtils.JsonToObject<List<string>>(firstExtendBundleJson);
//            foreach (var sp in buildinExtendAssetPathList){
//                string sourcePath = string.Empty;
//                string destPath   = string.Empty;
//                if (buildParameters.FileNameStyle == EFileNameStyle.HashName){
//                    if (manifest.TryGetPackageBundleByBundleName(sp, out PackageBundle packageBundle)){
//                        string hashSp = packageBundle.FileHash;
//                        sourcePath = $"{packageOutputDirectory}/{hashSp}.bundle";
//                        destPath   = $"{root}/{hashSp}.bundle";
//                    }
//                }
//                else{
//                    sourcePath = $"{packageOutputDirectory}/{sp}";
//                    destPath   = $"{root}/{sp}";
//                }

//                if (!File.Exists(sourcePath)){
//                    UnityEngine.Debug.Log($"[BuildingCmdPackABRes]无效包路径(Extend path):{sourcePath}".RedColor());
//                    continue;
//                }

//                EditorTools.CopyFile(sourcePath, destPath, true);
//            }

//            AssetDatabase.Refresh();
//            BuildLogger.Log($"首包资源拷贝完成!!!:{buildinRootDirectory}".GreenColor());
//        }

//        private void GetDependsPack(PackageManifest manifest, PackageBundle bundle, HashSet<PackageBundle> bundles, ref int depth){
//            //手动设置深度
//            if (depth >= 50){ return; }

//            depth++;

//            //获取依赖包
//            var depends = manifest.GetBundleAllDependencies(bundle);
//            if (null == depends || depends.Count <= 0){ return; }

//            foreach (var de in depends){
//                UnityEngine.Debug.Log($"[BuildingCmdPackABRes]获取包名:{de.BundleName}".YellowColor());
//                bundles.Add(de);
//                GetDependsPack(manifest, de, bundles, ref depth);
//            }
//        }

//        /// <summary>
//        /// Manifest部分资源是NoSerialized,需要手动Reload
//        /// </summary>
//        /// <param name="manifest"></param>
//        private void ManifestCheckReload(PackageManifest manifest){
//            if (null == manifest.AssetDic || manifest.AssetDic.Count <= 0){
//                manifest.AssetDic = new();
//                foreach (var pack in manifest.AssetList){
//                    //我们项目用的Address
//                    if (!manifest.AssetDic.TryAdd(pack.Address, pack)){
//                        manifest.AssetDic[pack.Address] = pack;
//                    }
//                }
//            }

//            if (null == manifest.BundleDic1 || manifest.BundleDic1.Count <= 0){
//                manifest.BundleDic1 = new();
//                foreach (var pack in manifest.BundleList){
//                    //我们项目用的Address
//                    if (!manifest.BundleDic1.TryAdd(pack.BundleName, pack)){
//                        manifest.BundleDic1[pack.BundleName] = pack;
//                    }
//                }
//            }
//        }

//    #endregion
//    }
//}