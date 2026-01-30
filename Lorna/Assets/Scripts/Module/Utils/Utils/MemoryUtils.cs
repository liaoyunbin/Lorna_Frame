using System;
using System.Runtime.InteropServices;
using LornaGame.ModuleExtensions;
using UnityEngine;
using UnityEngine.Profiling;

namespace LornaGame.ModuleExtensions.Utils{
    public class MemoryUtils : FrameWork_Tools.Singleton<MemoryUtils>{
        /// <summary> 
        /// 获取总内存
        /// PC
        /// </summary>
        /// <returns></returns>
        protected ulong GetWinTotalMemory(){
			//之前走kernel做法错误，linux各发行版内核都不同,用Unity 给的接口即可,底层给的MB,需要转回到B返回
			return (ulong)SystemInfo.systemMemorySize * 1024 * 1024;
        }

        /// <summary>
        /// 返回应用使用的内存
        /// PC
        /// </summary>
        /// <returns></returns>
        protected long GetWinUsedMemory(){
            try{
#if UNITY_STANDALONE_WIN
                return GetProcessMemoryWindows();
#elif UNITY_STANDALONE_LINUX
                return GetGetProcessMemoryLinux();
#endif
            }
            catch (Exception e){
#if PE_DEBUG
                UnityEngine.Debug.Log($"[PE进程获取失败]: 当前平台 => {UnityEngine.Application.platform},Error=>{e}");
#endif
                return Profiler.GetTotalReservedMemoryLong();
            }
        }

        /// <summary>
        /// 返回应用还未使用的空闲内存
        /// PC
        /// </summary>
        /// <returns></returns>
        protected long GetWinTotalUnusedMemory(){ return Profiler.GetTotalUnusedReservedMemoryLong(); }

    #region Public functions

        /// <summary>
        /// 获取当前应用占用系统内存比例
        /// </summary>
        public static double GetMemoryUsingRatio(){
            double ratio = (double)Instance.GetWinUsedMemory() / (double)Instance.GetWinTotalMemory();
#if PE_DEBUG
#if UNITY_STANDALONE_WIN
            UnityEngine.Debug.Log(
                                  $"[PE进程使用内存]:{FormatBytes(GetProcessMemoryWindows())} ,[总内存]:{FormatBytes((long)Instance.GetWinTotalMemory())} , [比例]:{ratio}"
                                 );
#elif UNITY_STANDALONE_LINUX
            UnityEngine.Debug.Log($"[PE进程使用内存]:{FormatBytes(GetGetProcessMemoryLinux())} ,[总内存]:{FormatBytes((long)Instance.GetWinTotalMemory())} , [比例]:{ratio}");
#endif
#endif
            return ratio;
        }

    #endregion

    #region 额外Dll支持

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string NATIVE_LIB = "WindowsMemoryMonitor";
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
    private const string NATIVE_LIB = "LinuxMemoryMonitor";
#else
    private const string NATIVE_LIB = "";
#endif

        // Windows API
        [DllImport(NATIVE_LIB, EntryPoint = "GetProcessMemory")]
        private static extern long GetProcessMemoryWindows();

        // Linux API
        [DllImport(NATIVE_LIB, EntryPoint = "GetProcessMemory")]
        private static extern long GetGetProcessMemoryLinux();

        private static readonly string[] s_suffixes ={ "B", "KB", "MB", "GB", "TB" };

        private static string FormatBytes(long bytes){
            int     counter = 0;
            decimal number  = bytes;
            while (Math.Round(number / 1024) >= 1 && counter < s_suffixes.Length - 1){
                number /= 1024;
                counter++;
            }

            return $"{number:n2} {s_suffixes[counter]}";
        }
    }

#endregion
}
