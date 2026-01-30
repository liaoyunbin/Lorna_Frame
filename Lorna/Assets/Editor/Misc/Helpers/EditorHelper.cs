using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace LornaGame.Editor{
    public static class EditorHelper{

        public static void ColorArea(Color color , Action callback)
        {
            GUI.backgroundColor = color;
            callback?.Invoke();
            GUI.backgroundColor = Color.white;
        }

        public static Vector2 ScrollView(Vector2 input, Action callback,bool showHorizontal = false, bool showVertical = false){
            input = GUILayout.BeginScrollView(input, showHorizontal, showVertical);
            callback?.Invoke();
            GUILayout.EndScrollView();
            return input;
        }

        public static void Horizontal(Action callback, GUIStyle style){
            EditorGUILayout.BeginHorizontal(style);
            callback?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        public static void Vertical(Action callback, GUIStyle style){
            EditorGUILayout.BeginVertical(style);
            callback?.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// List
        /// </summary>
        public static void DropDown<T>(int selectIndex, List<T> list, Action<T> onClick, float width = 120){
            bool       isOpenInList = selectIndex >= 0 && selectIndex < list.Count;
            string     openTypeName = isOpenInList ? list[selectIndex].ToString() : string.Empty;
            GUIContent guiContent   = new GUIContent(openTypeName);
            if (GUILayout.Button(guiContent, EditorStyles.toolbarDropDown, GUILayout.Width(width))){
                var menu = new GenericMenu();
                int len  = list.Count;
                for (var i = 0; i < len; ++i){
                    int editorSelect = i;
                    var single       = list[i];

                    void OnItemClick(){
                        onClick?.Invoke(single);
                        selectIndex = editorSelect;
                    }

                    menu.AddItem(new GUIContent(single.ToString()), isOpenInList && selectIndex == i, OnItemClick);
                }

                menu.ShowAsContext();
            }
        }

        public static void DropDown<T>(T selectElement, List<T> list, Action<T> onClick, float width = 120){
            bool       isOpenInList = null != selectElement && list.IndexOf(selectElement) >= 0;
            string     openTypeName = isOpenInList ? selectElement.ToString() : string.Empty;
            GUIContent guiContent   = new GUIContent(openTypeName);
            if (GUILayout.Button(guiContent, EditorStyles.toolbarDropDown, GUILayout.Width(width))){
                var menu = new GenericMenu();
                int len  = list.Count;
                for (var i = 0; i < len; ++i){
                    var single = list[i];

                    void OnItemClick(){
                        onClick?.Invoke(single);
                        selectElement = single;
                    }

                    menu.AddItem(new GUIContent(single.ToString()), isOpenInList && selectElement.Equals(single), OnItemClick);
                }

                menu.ShowAsContext();
            }
        }

        public static void DropDown<T, V>(
            T                          selectKey,
            Dictionary<T, V>           dict,
            Action<KeyValuePair<T, V>> onClick,
            float                      width      = 120,
            bool                       displayKey = false){
            bool       isOpenInList = dict.TryGetValue(selectKey, out V value);
            string     openTypeName = isOpenInList ? (displayKey ? selectKey.ToString() : value.ToString()) : string.Empty;
            GUIContent guiContent   = new GUIContent(openTypeName);
            if (GUILayout.Button(guiContent, EditorStyles.toolbarDropDown, GUILayout.Width(width))){
                var menu = new GenericMenu();
                foreach (var keyValuePair in dict){
                    string keyStr    = keyValuePair.Key.ToString();
                    string valueStr  = keyValuePair.Value.ToString();
                    string curKeyStr = displayKey ? keyStr : valueStr;

                    void OnItemClick(){
                        onClick?.Invoke(keyValuePair);
                        selectKey = keyValuePair.Key;
                    }

                    menu.AddItem(new GUIContent(curKeyStr), isOpenInList && selectKey.Equals(keyStr), OnItemClick);
                }

                menu.ShowAsContext();
            }
        }
    }
}
#endif
