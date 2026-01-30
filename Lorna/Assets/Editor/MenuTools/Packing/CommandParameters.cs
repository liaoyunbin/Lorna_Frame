using System.Collections.Generic;
using System.IO;
using System.Text;
using EscapeGame.Building.Config;
using LornaGame.ModuleExtensions;
using UnityEngine;

namespace EscapeGame.Building{
    public class CommandParameters : Dictionary<string, string>{
        private string _unityInputFile = null;
        protected string _UnityInputFile{
            get{
                if (null == _unityInputFile){
                    var dataPath = Application.dataPath;
                    var projPath = dataPath.Substring(0, dataPath.LastIndexOf("Assets"));
                    // _unityInputFile = Path.Combine(projPath, "Packing/unity_in.json");
                    _unityInputFile = Path.Combine(projPath, PackingConfIns.Ins.PackingInConf);
                }

                return _unityInputFile;
            }
        }

        public override string ToString(){
            StringBuilder sb = new StringBuilder(128);
            foreach (var kv in this){
                sb.Append("--");
                sb.Append(kv.Key);
                sb.Append(' ');
                if (kv.Value.Length > 0){
                    sb.Append(kv.Value);
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }

        public void SaveToFile(){
            Dictionary<string, string> dict    = new Dictionary<string, string>(this);
            string                     content = JsonUtils.ObjectToJson(dict);
            // string                     content = JsonUtility.ToJson(dict);
            File.WriteAllText(_UnityInputFile, content, new UTF8Encoding(false));
        }

        public static CommandParameters LoadFromFile(){
            CommandParameters sp = new CommandParameters();
            if (File.Exists(sp._UnityInputFile)){
                var content = File.ReadAllText(sp._UnityInputFile, new UTF8Encoding(false));
                var dict    = JsonUtils.JsonToObject<Dictionary<string, string>>(content); //EscapeGame.Utils
                // var dict    = JsonUtility.FromJson<Dictionary<string, string>>(content);
                if (dict != null){
                    foreach (var kv in dict){
                        sp.Add(kv.Key, kv.Value);
                    }
                }
            }

            return sp;
        }
    }
}