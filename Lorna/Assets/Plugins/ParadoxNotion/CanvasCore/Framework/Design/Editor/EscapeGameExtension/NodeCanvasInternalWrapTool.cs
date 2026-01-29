using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace NodeCanvas.Editor{
    public static class NodeCanvasInternalWrapTool{
        /// <summary>
        /// Key - value
        /// </summary>
        private static Dictionary<SystemLanguage, Dictionary<string, string>> s_internalLanguageWrapDic = new();

        /// <summary>
        /// Value - key
        /// </summary>
        private static Dictionary<SystemLanguage, Dictionary<string, string>> s_internalLanguageWrapReverseDic = new();

    #region Public parasms

        /// <summary>
        /// 当前所有语言
        //TODO: 先手动加,有正式需求了再补充做法
        /// </summary>
        public static HashSet<SystemLanguage> LanSet = new(){ SystemLanguage.ChineseSimplified, SystemLanguage.English };

        /// <summary>
        /// 当前选择语言(默认简中)
        /// </summary>
        public static SystemLanguage Language = SystemLanguage.ChineseSimplified;

    #endregion

        /// <summary>
        /// 根据多语言Key获取文本
        /// </summary>
        public static string GetLangText(SystemLanguage lan, string key){
            if (string.IsNullOrEmpty(key)){
                return string.Empty;
            }

            if (s_internalLanguageWrapDic == null){
                return string.IsNullOrEmpty(key) ? string.Empty : key;
            }

            if (s_internalLanguageWrapDic.TryGetValue(lan, out var value)){
                if (value.TryGetValue(key, out var rel)){
                    return rel;
                }

                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 根据多语言文本获取key
        /// </summary>
        public static string GetLangKey(SystemLanguage lan, string text){
            if (string.IsNullOrEmpty(text)){
                return string.Empty;
            }

            if (s_internalLanguageWrapReverseDic == null){
                return string.IsNullOrEmpty(text) ? string.Empty : text;
            }

            if (s_internalLanguageWrapReverseDic.TryGetValue(lan, out var value)){
                if (value.TryGetValue(text, out var rel)){
                    return rel;
                }

                return string.Empty;
            }

            return string.Empty;
        }

    #region Editor postprocessor

        /// <summary>
        /// 编辑器完成重载
        /// 每次重载加载多语言配置到内存
        /// TODO:程序集有修改或者额外ADF已创建的情况，需要单独过来修改
        /// </summary>
        [InitializeOnLoad]
        public class EngineInternalCompiling : AssetPostprocessor{
            [UnityEditor.Callbacks.DidReloadScripts]
            static void AllScriptsReloaded(){
                s_internalLanguageWrapDic        = new();
                s_internalLanguageWrapReverseDic = new();

                ////反射获取
                try{
                    //旧做法，反射创建LanManager
                    // var  csharpAb  = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
                    // Type type      = csharpAb.GetType("EscapeGame.Manager.LanguageManager");
                    // var  files     = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    // var  insLanMgr = Activator.CreateInstance(type); //跨程序集理论不可直接createInstance,但是Editor内Exception也有Object的，只要能给反射获取实例内容，这里就不纠结先用着
                    // var  val       = files.First(t => t.Name == "_multiLan").GetValue(insLanMgr);
                    // s_internalLanguageWrapDic = val as Dictionary<SystemLanguage, Dictionary<string, string>>;
                    // //Reverse 
                    // foreach (var subDic in s_internalLanguageWrapDic){
                    //     Dictionary<string, string> reverseDict = new();
                    //     foreach (var pair in subDic.Value){
                    //         reverseDict.TryAdd(pair.Value, pair.Key);
                    //     }
                    //
                    //     s_internalLanguageWrapReverseDic.Add(subDic.Key, reverseDict);
                    // }
                    if (EditorApplication.isCompiling){
                        return;
                    }

                    //新做法,直接HookJson即可
                    //每新加一个语言都得加初始化
                    string jsonPath       = "Assets/Game/Res/AssetBundleRes/Config/tblanguage.json";
                    var    textAssets     = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
                    if (null == textAssets){
                        Debug.Log($"[NodeCanvasInternalWrapTool] 加载多语言配置错误,请检查配置路径是否存在 => {jsonPath}");
                        return;
                    }

                    string jsonStr        = textAssets.text;
                    var    deserializeObj = JsonConvert.DeserializeObject<List<TempLanObj>>(jsonStr);

                    //cn
                    Dictionary<string, string> cnRe = new();
                    Dictionary<string, string> cn   = new();
                    //tc
                    Dictionary<string, string> tcRe = new();
                    Dictionary<string, string> tc   = new();
                    //en
                    Dictionary<string, string> enRe = new();
                    Dictionary<string, string> en   = new();
                    //jp
                    Dictionary<string, string> jpRe = new();
                    Dictionary<string, string> jp   = new();
                    //de
                    Dictionary<string, string> deRe = new();
                    Dictionary<string, string> de   = new();
                    //kr
                    Dictionary<string, string> krRe = new();
                    Dictionary<string, string> kr   = new();
                    //fr
                    Dictionary<string, string> frRe = new();
                    Dictionary<string, string> fr   = new();
                    foreach (var subDic in deserializeObj){
                        cn.Add(subDic.id, subDic.cn);
                        tc.Add(subDic.id, subDic.tc);
                        en.Add(subDic.id, subDic.en);
                        jp.Add(subDic.id, subDic.jp);
                        de.Add(subDic.id, subDic.de);
                        kr.Add(subDic.id, subDic.kr);
                        fr.Add(subDic.id, subDic.fr);

                        //Re
                        if (!cnRe.TryAdd(subDic.cn, subDic.id)){
                            cnRe[subDic.cn] = subDic.id;
                        }

                        if (!tcRe.TryAdd(subDic.tc, subDic.id)){
                            tcRe[subDic.tc] = subDic.id;
                        }

                        if (!enRe.TryAdd(subDic.en, subDic.id)){
                            enRe[subDic.en] = subDic.id;
                        }

                        if (!jpRe.TryAdd(subDic.jp, subDic.id)){
                            jpRe[subDic.jp] = subDic.id;
                        }

                        if (!deRe.TryAdd(subDic.de, subDic.id)){
                            deRe[subDic.de] = subDic.id;
                        }
                        
                        if (!krRe.TryAdd(subDic.kr, subDic.id)){
                            krRe[subDic.kr] = subDic.id;
                        }
                        
                        if (!frRe.TryAdd(subDic.fr, subDic.id)){
                            frRe[subDic.fr] = subDic.id;
                        }
                    }

                    //Forward
                    s_internalLanguageWrapDic.Add(SystemLanguage.ChineseSimplified, cn);
                    s_internalLanguageWrapDic.Add(SystemLanguage.ChineseTraditional, tc);
                    s_internalLanguageWrapDic.Add(SystemLanguage.English, en);
                    s_internalLanguageWrapDic.Add(SystemLanguage.Japanese, jp);
                    s_internalLanguageWrapDic.Add(SystemLanguage.German, de);
                    s_internalLanguageWrapDic.Add(SystemLanguage.Korean, kr);
                    s_internalLanguageWrapDic.Add(SystemLanguage.French, fr);
                    //Reverse
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.ChineseSimplified, cnRe);
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.ChineseTraditional, tcRe);
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.English, enRe);
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.Japanese, jpRe);
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.German, deRe);
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.Korean, krRe);
                    s_internalLanguageWrapReverseDic.Add(SystemLanguage.French, frRe);
                }
                catch (Exception e){
                    Debug.LogError(e);
                }
            }
        }

        public class TempLanObj{
            public string id;
            public string cn;
            public string tc;
            public string en;
            public string jp;
            public string de;
            public string kr;
            public string fr;
        }

    #endregion
    }
}
#endif