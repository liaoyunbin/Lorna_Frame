
using System.Collections.Generic;
using System;

namespace FrameWork_Tools
{
    /// <summary>
    /// 包装层管理类
    /// </summary>
    public class WrapMgr : Singleton<WrapMgr>
    {
        private static T GetMgr<T>() where T : class,IMgr
        {
            List<System.Type> implementingTypes = ADFUtils.GetImplementingTypes(typeof(T)) as List<System.Type>;
            if (implementingTypes is not { Count: > 0 })
            {
                throw new Exception($"没有找到 {typeof(T)} 继承类,加载错误:{implementingTypes}");
            }

            return Activator.CreateInstance(implementingTypes[0]) as T;
        }

        private static ICameraManager m_CameraManager;
        private static IAssetsManager m_AssetManager;
        private static ITaskManager m_TaskManager;

        public static ICameraManager cameraManager
        {
            get
            {
                m_CameraManager = m_CameraManager ?? GetMgr<ICameraManager>();
                return m_CameraManager;
            }
        }
        public static IAssetsManager assetManager
        {
            get
            {
                m_AssetManager = m_AssetManager ?? GetMgr<IAssetsManager>();
                return m_AssetManager;
            }
        }
        public static ITaskManager taskManager
        {
            get
            {
                m_TaskManager = m_TaskManager ?? GetMgr<ITaskManager>();
                return m_TaskManager;
            }
        }
    }
}
