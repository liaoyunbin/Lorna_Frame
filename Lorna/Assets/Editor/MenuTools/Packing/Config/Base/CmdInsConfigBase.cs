using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace EscapeGame.Building.Config{
    internal abstract class CmdInsConfigBase<T>{
    #region Instance

        private static T _instance;

        internal static T Ins{
            get{
                if (null == _instance){
                    if (!GetPath(out string path)){
                        return default(T);
                    }

                    if (!File.Exists(path)){
                        _instance = Activator.CreateInstance<T>();
                        File.WriteAllText(path, JsonUtility.ToJson(_instance));
                    }
                    else{
                        string cnfStr = File.ReadAllText(path, new UTF8Encoding(false));
                        _instance = JsonUtility.FromJson<T>(cnfStr);
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 立即存储一次
        /// </summary>
        internal static void SaveImmediately(){
            if (GetPath(out string path)){
                File.WriteAllText(path, JsonUtility.ToJson(_instance));
            }
        }

    #region private functions

        private static bool GetPath(out string path){
            path = string.Empty;
            Type               type      = typeof(T);
            InsConfigAttribute attribute = type.GetCustomAttributes(typeof(InsConfigAttribute), true)?[0] as InsConfigAttribute;
            if (null == attribute){
                UnityEngine.Debug.LogError($"[CmdInsConfigBase]请检查Config的Attribute正确配置");
                return false;
            }

            path = attribute.JsonPath;
            return true;
        }

    #endregion

    #endregion
    }
}