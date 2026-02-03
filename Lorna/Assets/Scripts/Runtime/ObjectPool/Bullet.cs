using UnityEngine;

namespace LornaGame.Runtime
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        private Rigidbody _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void OnGet()
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce(transform.forward * 1000f);
        }

        public void OnReturn()
        {
            _rb.velocity = Vector3.zero;
        }
    }

    public interface IPoolable
    {
        void OnGet();   // 被取出时的初始化
        void OnReturn(); // 被回收时的清理
    }
}
