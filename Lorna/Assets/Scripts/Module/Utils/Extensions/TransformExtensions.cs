using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LornaGame.ModuleExtensions
{
    public static class TransformExtensions
    {

        //获取场景的方向地板检测 最近检测点
        public static bool TryGetRaycastFirstScene(this Transform target, Vector3 dirVector3, out Vector3 result)
        {
            Vector3 point = target.position;
            Ray ray = new Ray(point, dirVector3);
            int mask = LayerMask.GetMask("Scene");
            result = Vector3.zero;
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore);

            float distance = 0;
            bool resB = false;
            foreach (RaycastHit hit in hits)
            {

                var dis = Vector3.Distance(hit.point, point);
                if (distance == 0)
                {
                    result = hit.point;
                    distance = dis;
                }

                if (distance > dis)
                {
                    distance = dis;
                    result = hit.point;
                }
                resB = true;
            }

            return resB;
        }

        public static void DestroyChildren(this Transform target)
        {
            for (int i = target.childCount - 1; i >= 0; i--)
                Object.Destroy(target.GetChild(i).gameObject);
        }

        public static void ResetTransformation(this Transform target)
        {
            target.position = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }

        public static Transform GetChildByName(this Transform target, string childName)
        {
            foreach (Transform child in target)
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            // throw new KeyNotFoundException();
            Logs.LogError($"无法正确获取到Transform的ChildName，请检查名字是否匹配!");
            return null;
        }


        public static IEnumerable<Transform> GetChildren(this Transform target)
        {
            foreach (Transform child in target)
                yield return child;
        }

        public static IEnumerable<Transform> Traverse(this Transform target)
        {
            yield return target;
            foreach (Transform x in target)
                foreach (Transform y in x.Traverse())
                    yield return y;
        }

        public static IEnumerable<Transform> Ancestors(this Transform target)
        {
            yield return target;
            if (target.parent == null)
                yield break;
            foreach (Transform x in target.parent.Ancestors())
                yield return x;
        }


    }
}