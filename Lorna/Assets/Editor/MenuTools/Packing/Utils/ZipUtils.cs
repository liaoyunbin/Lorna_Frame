using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace EscapeGame.Building.Utils{
    public static class ZipUtils{
        /// <summary>
        /// 分片数据大小
        /// </summary>
        private static int ARCH_SEGEMENT_SIZE = 1024 * 1024 * 100; //100MB写一次

        /// <summary>
        /// 标准后缀
        /// </summary>
        private const string NORMAL_SUFFIX = ".zip";

        /// <summary>
        /// 特殊处理后缀
        /// </summary>
        private const string DIRECTORY_SUFFIX = "//";

    #region Public functions

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="outputPath"></param>
        /// <param name="compressedLevel"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool Zip(string filePath, string outputPath, int compressedLevel, string pwd){
            if (Directory.Exists(filePath)){
                return ZipFileDirectory(filePath, outputPath, compressedLevel, pwd);
            }
            else if (File.Exists(filePath)){
                return ZipFile(filePath, outputPath, compressedLevel, pwd);
            }

            return false;
        }

    #region 解压

        ///<summary>  
        ///功能：解压zip格式的文件。  
        ///</summary>  
        ///<param name="zipFilePath">压缩文件路径，全路径格式</param>  
        ///<param name="unZipDir">解压文件存放路径,全路径格式，为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
        ///<param name="err">出错信息</param>  
        ///<returns>解压是否成功</returns>  
        public static bool UnZip(string zipFilePath, string unZipDir){
            if (zipFilePath == string.Empty){
                throw new FileNotFoundException("[ZipUtils] 压缩文件不不能为空！");
            }

            if (!File.Exists(zipFilePath)){
                throw new FileNotFoundException("[ZipUtils] 压缩文件: " + zipFilePath + " 不存在!");
            }

            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (unZipDir == string.Empty) unZipDir             =  zipFilePath.Replace(Path.GetFileName(zipFilePath), "");
            if (!unZipDir.EndsWith(DIRECTORY_SUFFIX)) unZipDir += DIRECTORY_SUFFIX;
            if (!Directory.Exists(unZipDir)){ Directory.CreateDirectory(unZipDir); }

            byte[] buffer = new byte[ARCH_SEGEMENT_SIZE];
            try{
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath))){
                    while (s.GetNextEntry() is{ } theEntry){
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName      = Path.GetFileName(theEntry.Name);
                        if (directoryName != null && !directoryName.EndsWith(DIRECTORY_SUFFIX)){
                            directoryName += DIRECTORY_SUFFIX;
                            if (directoryName.Length > 0 && !Directory.Exists(unZipDir + directoryName)){
                                Directory.CreateDirectory(unZipDir + directoryName);
                            }
                        }

                        if (string.IsNullOrEmpty(fileName)){ continue; }

                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name)){
                            int sourceBytes;
                            do{
                                sourceBytes = s.Read(buffer, 0, buffer.Length);
                                streamWriter.Write(buffer, 0, sourceBytes);
                            }
                            while (sourceBytes > 0);
                        }
                    }
                }
            }
            catch (Exception ex){
                throw new Exception(($"[ZipUtils] 压缩文件夹错误:{ex.Message}"));
            }

            return true;
        } //解压结束 

    #endregion

    #endregion

    #region Private functions

    #region 压缩文件

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="filePath">要进行压缩的文件名</param>
        /// <param name="outputFilePath">压缩后生成的压缩文件名，如果为空则文件名为待压缩的文件名加上.rar</param>
        /// <param name="compressLevel">压缩等级0-9</param>
        /// <param name="pwd"></param>
        /// <returns>压缩是否成功</returns>
        private static bool ZipFile(string filePath, string outputFilePath, int compressLevel, string pwd){
            //如果文件没有找到，则报错
            if (!File.Exists(filePath)){
                throw new FileNotFoundException("指定要压缩的文件: " + filePath + " 不存在!");
            }

            if (string.IsNullOrEmpty(outputFilePath)){
                //如果为空则文件名为待压缩的文件名加上.rar
                outputFilePath = filePath + NORMAL_SUFFIX;
            }

            byte[] buffer = new byte[ARCH_SEGEMENT_SIZE];
            using (ZipOutputStream stream = new ZipOutputStream(File.Create(outputFilePath))){
                stream.SetLevel(compressLevel); //设置压缩等级
                stream.Password = pwd;          //设置压缩密码
                var entry = new ZipEntry(Path.GetFileName(filePath)){ DateTime = DateTime.Now };
                stream.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(filePath)){
                    int sourceBytes;
                    do{
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, sourceBytes);
                    }
                    while (sourceBytes > 0);
                }

                stream.Finish();
                stream.Close();
            }

            return true;
        }

    #endregion

    #region 递归压缩文件夹

        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="folderPath">待压缩的文件夹，全路径格式</param>
        /// <param name="outputFilePath">压缩后的文件名，全路径格式，如果为空则文件名为待压缩的文件名加上.rar</param>
        /// <param name="compressedLevel">压缩等级0-9</param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private static bool ZipFileDirectory(string folderPath, string outputFilePath, int compressedLevel, string pwd){
            if (!Directory.Exists(folderPath)){ return false; }

            if (outputFilePath == string.Empty){
                //如果为空则文件名为待压缩的文件名加上.rar
                outputFilePath = folderPath + NORMAL_SUFFIX;
            }

            ZipOutputStream s = new ZipOutputStream(File.Create(outputFilePath));
            s.SetLevel(compressedLevel);
            if (!string.IsNullOrEmpty(pwd.Trim())) s.Password = pwd.Trim();
            bool rel                                          = ZipFileDirectory(folderPath, s, "");
            s.Finish();
            s.Close();
            return rel;
        }

        ///<summary>
        ///递归压缩文件夹方法
        ///</summary>
        ///<param name="folderPath"></param>
        ///<param name="s"></param>
        ///<param name="parentFolder"></param>
        private static bool ZipFileDirectory(string folderPath, ZipOutputStream s, string parentFolder){
            //
            if (!Directory.Exists(folderPath)){
                Directory.CreateDirectory(folderPath);
            }

            //加上 “/” 才会当成是文件夹创建
            ZipEntry   entry = null;
            FileStream fs    = null;
            try{
                entry = new ZipEntry(Path.Combine(parentFolder, Path.GetFileName(folderPath) + "/"));
                s.PutNextEntry(entry);
                s.Flush();
                //先压缩文件，再递归压缩文件夹
                string[] directoryFiles = Directory.GetFiles(folderPath);
                foreach (string file in directoryFiles){
                    //打开压缩文件
                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[ARCH_SEGEMENT_SIZE];
                    entry          = new ZipEntry(Path.Combine(parentFolder, Path.GetFileName(folderPath) + "/" + Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size     = fs.Length;
                    s.PutNextEntry(entry);
                    int sourceBytes;
                    do{
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, sourceBytes);
                    }
                    while (sourceBytes > 0);
                }
            }
            catch (Exception e){
                UnityEngine.Debug.LogError($"[ZipUtils] 压缩文件夹错误:{e}");
                return false;
            }
            finally{
                if (fs != null){
                    fs.Close();
                    fs = null;
                }

                if (entry != null){ entry = null; }

                GC.Collect();
                GC.Collect(1);
            }

            //递归子文件夹,继续此步骤
            var folders = Directory.GetDirectories(folderPath);
            foreach (string folder in folders){
                if (!ZipFileDirectory(folder, s, Path.Combine(parentFolder, Path.GetFileName(folderPath)))){ return false; }
            }

            return true;
        }

    #endregion

    #endregion
    }
}