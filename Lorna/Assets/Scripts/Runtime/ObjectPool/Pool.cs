namespace LornaGame.Runtime
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Pool
    {
        public GameObject Prefab;
        public int InitialSize = 10;
        private Queue<GameObject> _available = new Queue<GameObject>();

        public Pool(GameObject prefab, int initialSize)
        {
            Prefab = prefab;
            InitialSize = initialSize;
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < InitialSize; i++)
            {
                AddObject();
            }
        }

        private GameObject AddObject(bool isActive = false)
        {
            GameObject obj = Object.Instantiate(Prefab);
            obj.SetActive(isActive);
            _available.Enqueue(obj);
            return obj;
        }

        public GameObject Get()
        {
            if (_available.Count == 0)
                AddObject();

            GameObject obj = _available.Dequeue();
            obj.SetActive(true);

            // 调用自定义初始化逻辑
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnGet();

            return obj;
        }

        public void Return(GameObject obj)
        {
            // 调用自定义回收逻辑
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnReturn();

            obj.SetActive(false);
            _available.Enqueue(obj);
        }
    }
}