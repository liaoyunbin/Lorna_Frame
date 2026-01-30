using System.Text;
using UnityEngine;

namespace LornaGame.ModuleExtensions{
    public static class GameObjectUtils{
        public static void TransformZero(this GameObject gameObject, bool identityRotate = true){
            Transform tr = gameObject.transform;
            TransformZero(tr, identityRotate);
        }

        public static void TransformZero(this Transform tr, bool identityRotate = true){
            tr.localPosition = Vector3.zero;
            tr.localScale    = Vector3.one;
            if (identityRotate){
                tr.localRotation = Quaternion.identity;
            }
        }

        public static GameObject CreateGameObject(string name = "", bool isStatic = false){
            GameObject obj = new GameObject(name);
            TransformZero(obj, false);
            obj.isStatic = isStatic;
            return obj;
        }

        public static RectTransform CreateGameObject(this RectTransform parent, string name = "", bool isStatic = false){
            GameObject obj       = new GameObject(name);
            var        rectTrans = obj.GetOrAddComponent<RectTransform>();
            rectTrans.localScale = Vector3.one;
            rectTrans.position   = parent.position;
            rectTrans.SetParent(parent);
            obj.isStatic = isStatic;
            return rectTrans;
        }

        public static GameObject CreateGameObject(this Transform parent, string name = "", bool isStatic = false){
            GameObject obj = new GameObject(name);
            obj.transform.localScale = Vector3.one;
            obj.transform.position   = parent.position;
            obj.transform.SetParent(parent);
            obj.isStatic = isStatic;
            return obj;
        }

        public static T CreateGameObject<T>(this Transform parent, string name = "", bool isStatic = false)
            where T : Component{
            GameObject obj       = new GameObject(name);
            T          component = obj.GetOrAddComponent<T>();
            obj.transform.localScale = Vector3.one;
            obj.transform.position   = parent.position;
            obj.transform.SetParent(parent);
            obj.isStatic = isStatic;
            return component;
        }

        public static T CreateGameObject<T>(string name = "", bool isStatic = false) where T : Component{
            GameObject result = new GameObject(name);
            TransformZero(result);
            result.isStatic = isStatic;
            T component = result.GetOrAddComponent<T>();
            return component;
        }

        public static T CreateGameObjectAndComponent<T>(string name = "", UnityEngine.Transform parent = null, bool isStatic = false)
            where T : Component{
            GameObject obj = new GameObject(name);
            TransformZero(obj);
            obj.transform.SetParent(parent);
            obj.isStatic = isStatic;
            T result = obj.GetOrAddComponent<T>();
            return result;
        }

        /// <summary>
        /// 简单遍历
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="onCheck">检测到时</param>
        public static void Ergodic(this Transform parent, System.Action<Transform> onCheck){
            int childrenCount = parent?.childCount ?? 0;
            for (int i = 0; i < childrenCount; i++){
                Transform tr = parent.GetChild(i);
                onCheck?.Invoke(tr);
            }
        }

        /// <summary>
        /// 简单遍历
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="onCheck">检测目标节点时,break当前循环</param>
        public static void Ergodic(this Transform parent, System.Func<Transform, bool> onCheck){
            int childrenCount = parent?.childCount ?? 0;
            for (int i = 0; i < childrenCount; i++){
                Transform tr = parent.GetChild(i);
                if (onCheck.Invoke(tr)){
                    break;
                }
            }
        }

        public static void DestroyParent(this GameObject parent, bool detachChildren = false){
            if (detachChildren){
                parent.transform.DetachChildren();
            }

            DestroyObj(parent);
        }

        public static void DestroyKids(this GameObject parent){
            int childCount = parent.transform?.childCount ?? 0;
            for (int i = 0; i < childCount; i++){
                DestroyObj(parent.transform.GetChild(i));
            }
        }

        public static void DestroyKids(this Transform parent){
            foreach (Transform child in parent){
                DestroyObj(child);
            }
        }

        public static void DestroyObj(GameObject obj){
            if (Application.isPlaying){
                GameObject.Destroy(obj);
            }
            else{
                GameObject.DestroyImmediate(obj);
            }
        }

        public static void DestroyObj(Transform obj){
            if (Application.isPlaying){
                GameObject.Destroy(obj.gameObject);
            }
            else{
                GameObject.DestroyImmediate(obj.gameObject);
            }
        }

        public static void DestroyObj<T>(T obj) where T : Component{
            if (Application.isPlaying){
                GameObject.Destroy(obj.gameObject);
            }
            else{
                GameObject.DestroyImmediate(obj.gameObject);
            }
        }

        private static StringBuilder st = new StringBuilder();
        public static string GetGlobalPathName(GameObject _go, string name)
        {
            st.Clear();
            st.Append(_go.scene.name);
            st.Append('_');
            st.Append(_go.GetAllPath());
            st.Append('_');
            st.Append(name);
            //去除字符中的 空格 () 符号
            st = st.Replace(' ', '_').Replace('(', '_').Replace(')', '_').Replace('.', '_').Replace('~', '_');
            return st.ToString();
        }
    }
}