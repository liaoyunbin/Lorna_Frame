
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
namespace LornaGame.ModuleExtensions.ArchiveModule
{
    public class ArchivePath
    {
        /// <summary>
        ///存档插槽
        /// </summary>
        public int slotId { get; private set; }
        /// <summary>
        /// 存档文件夹路径
        /// </summary>
        public string ArchiveDirectoryPath { get; private set; }
        /// <summary>
        /// 临时存档路径
        /// </summary>
        public string TemporaryArchivePath { get; private set; }
        /// <summary>
        /// 永久存档路径
        /// </summary>
        public string PermanentArchivePath { get; private set; }
        /// <summary>
        /// 全局存档路径
        /// </summary>
        public string GlobalArchivePath { get; private set; }
        /// <summary>
        /// 当前数据路径
        /// </summary>
        public string NowArchivePath { get; private set; }

        /// <summary>
        /// 文件夹前缀
        /// </summary>
        private const string DirectoryPathPre = "SaveFiles";

        public void InitPath(string _path,int _slotId)
        {
            slotId = _slotId;
            ArchiveDirectoryPath = string.Concat(_path, "/Clound/", DirectoryPathPre, _slotId);
            TemporaryArchivePath = string.Concat(ArchiveDirectoryPath, "/TemporaryArchive.json");
            PermanentArchivePath = string.Concat(ArchiveDirectoryPath + "/PermanentArchive.json");
            GlobalArchivePath    = string.Concat(ArchiveDirectoryPath, "/GlobalArchive.json");
            NowArchivePath       = string.Concat(ArchiveDirectoryPath, "/NowArchive.json");
        }

    }
}
