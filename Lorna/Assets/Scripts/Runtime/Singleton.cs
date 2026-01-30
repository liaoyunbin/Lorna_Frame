namespace FrameWork_Tools
{// 泛型单例基类，约束 T 必须具有无参构造函数
    public abstract class Singleton<T> where T : class, new()
    {
        // 静态私有实例
        private static T _instance;
        // 线程锁，确保多线程安全
        private static readonly object _lock = new object();

        // 公开的静态访问点
        public static T Instance
        {
            get
            {
                // 双重检查锁定，提高性能的同时保证线程安全
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T(); // 创建唯一实例
                        }
                    }
                }
                return _instance;
            }
        }

        // 受保护的构造函数，防止外部实例化
        protected Singleton() { }

        // 可选：提供卸载方法，用于重置单例（如在游戏退出时）
        public static void Unload()
        {
            _instance = null;
        }
    }
}