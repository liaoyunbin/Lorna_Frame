using UnityEngine;

namespace LornaGame.Runtime
{
    public class BulletTest : MonoBehaviour
    {
        
        public GameObject bulletPrefab;
        void Test()
        {
            // 生成子弹
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.position = transform.position;

            // 回收子弹（如碰撞后）
            ObjectPool.Instance.ReturnObject(bullet);
        }
    }
}
