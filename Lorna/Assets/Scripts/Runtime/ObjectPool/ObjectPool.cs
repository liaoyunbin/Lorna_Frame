using System.Collections.Generic;
using UnityEngine;
using LornaGame.ModuleExtensions;

namespace LornaGame.Runtime
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        private Dictionary<GameObject, Pool> _pools = new Dictionary<GameObject, Pool>();
        private Dictionary<GameObject, GameObject> _instanceToPrefabMap = new Dictionary<GameObject, GameObject>();

        public GameObject GetObject(GameObject prefab)
        {
            if (!_pools.ContainsKey(prefab))
                _pools[prefab] = new Pool(prefab, 10);

            GameObject obj = _pools[prefab].Get();
            _instanceToPrefabMap[obj] = prefab;
            return obj;
        }

        public void ReturnObject(GameObject obj)
        {
            if (_instanceToPrefabMap.ContainsKey(obj))
            {
                GameObject prefab = _instanceToPrefabMap[obj];
                _pools[prefab].Return(obj);
                _instanceToPrefabMap.Remove(obj);
            }
            else
            {
                Object.Destroy(obj);
            }
        }
    }
}

