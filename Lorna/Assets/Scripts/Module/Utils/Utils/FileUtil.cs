using LornaGame.ModuleExtensions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace LornaGame.ModuleExtensions.Utils
{
    public class FileUtil
    {
        private const string ErrorTips = "ERROR";
        private static void SetContent(string datapath, ref string content)
        {
            try
            {
                content = FileUtil.ReadByJson(datapath);
            }
            catch (System.Exception ex)
            {
                Logs.LogError("未知错误:文件路径{0}，报错信息{1}", datapath, ex.Message);
                content = ErrorTips;
            }
        }
        public static bool LoadByJson<T>(string datapath, JsonSerializerSettings _setting, ref T data) where T : class, new()
        {
            string content = string.Empty;
            SetContent(datapath, ref content);
            bool isValid = content != ErrorTips;
            data = (isValid && !content.Equals(string.Empty)) ?
                JsonUtils.JsonToObject<T>(content, _setting) : new T();
            return isValid;
        }
        public static void WriteToJson(string _path, string _content, bool _isAtomic = true)
        {
            var director = Path.GetDirectoryName(_path);
            if (!Directory.Exists(director))
            {
                Directory.CreateDirectory(director);
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(_content);
            byte[] encryptedBytes = Encrypt(bytes);

            // 生成临时文件路径（在原文件名后加 .tmp）
            string tempPath = _path.Replace(".json", "") + ".tmp";
 
            // 1. 先写入临时文件
            using (FileStream fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(encryptedBytes, 0, encryptedBytes.Length);
                // 确保数据落盘（可选，视安全性需求而定）
                fs.Flush(true);
            }

            // 2. 将临时文件移动/替换为目标文件（这是原子操作）
            if (File.Exists(_path))
            {
                File.Replace(tempPath, _path, null);
            }
            else
            {
                File.Move(tempPath, _path);
            }
        }

        public static string ReadByJson(string _path)
        {
            if (File.Exists(_path))  //判断这个路径里面是否为空
            {
                byte[] bytes;
                //var bytes=File.ReadAllBytes(_path);
                using (FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // 根据文件长度初始化字节数组
                    bytes = new byte[fs.Length];
                    // 读取字节：将数据从流读取到缓冲区中
                    int bytesRead = fs.Read(bytes, 0, bytes.Length);
                }
                byte[] decryptedBytes = Decrypt(bytes);
                return System.Text.Encoding.UTF8.GetString(decryptedBytes);
            }
            throw new ArgumentException(_path + "路径上未找到文件");
        }

        /// <summary>
        /// 写入非加密文件
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_content"></param>
        public static void WriteNoEncryptionToJson(string _path, string _content)
        {
            var director = Path.GetDirectoryName(_path);
            if (!Directory.Exists(director))
            {
                Directory.CreateDirectory(director);
            }
            StreamWriter sw = new StreamWriter(_path, false); //创建一个写入流
            sw.Write(_content);//将dateStr写入
            sw.Close();//关闭流
        }

        /// <summary>
        /// 获取非加密文件
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static string ReadNoEncryptionByJson(string _path)
        {
            if (File.Exists(_path))  //判断这个路径里面是否为空
            {
                StreamReader sr = new StreamReader(_path, false);//创建读取流;
                string jsonStr = sr.ReadToEnd();//使用方法ReadToEnd（）遍历的到保存的内容
                sr.Close();
                return jsonStr;
            }
            Logs.LogError(_path + "路径上未找到文件");
            return string.Empty;
        }


        // ！重要：请务必更改此密钥，并使用安全的方式存储和传输它！
        // 密钥长度必须是 16 (AES-128), 24 (AES-192), 或 32 (AES-256) 字节
        private static readonly string EncryptionKey = "Your16ByteKey123"; // 示例密钥，请修改！
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("YourSaltHere1234"); // 额外的“盐”，增加安全性
        private static byte[] Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
                throw new ArgumentException("明文数据不能为空！");

            using (Aes aesAlgo = Aes.Create())
            {
                // 使用 Rfc2898DeriveBytes 从密码和盐中派生密钥，更安全
                Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                aesAlgo.Key = keyDeriver.GetBytes(32); // 获取256位密钥
                aesAlgo.IV = keyDeriver.GetBytes(16);  // 获取128位初始化向量

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlgo.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        /// <summary>
        /// 解密字节数组
        /// </summary>
        /// <param name="cipherBytes">加密后的密文字节数组</param>
        /// <returns>解密后的原始字节数组</returns>
        private static byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
                throw new ArgumentException("密文数据不能为空！");

            using (Aes aesAlgo = Aes.Create())
            {
                Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                aesAlgo.Key = keyDeriver.GetBytes(32);
                aesAlgo.IV = keyDeriver.GetBytes(16);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlgo.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    return msDecrypt.ToArray();
                }
            }
        }
    }
}
