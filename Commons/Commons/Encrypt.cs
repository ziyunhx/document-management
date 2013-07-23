using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Commons
{
    /// <summary> 
    /// EncryptAndDecode 的摘要说明。 
    /// </summary> 
    public class EncryptAndDecode
    {
        public EncryptAndDecode()
        {
            // 
            // TODO: 在此处添加构造函数逻辑 
            // 
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="url">文件url</param>
        /// <returns>是否加密成功</returns>
        public static bool DesEncryptFile(string url)
        {
            //ReadBinaryFiles(url);
            return true;
        }


        /// <summary> 
        /// 加密 
        /// </summary> 
        public static string DesEncrypt(string strValue)
        {
            byte[] byKey = { 0x12, 0x34, 0x56, 0x78, 0x90, 0x13, 0x57, 0x90 };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0x22, 0x44, 0x66 };
            try
            {
                //创建一个DES算法的加密类 
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //将字符串转换成字节 
                byte[] YourInputStorage = Encoding.UTF8.GetBytes(strValue);
                //在内存中创建一个支持存储区的流 
                MemoryStream ms = new MemoryStream();
                //CryptoStream对象的作用是将数据流连接到加密转换的流 
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                //将字节数组中的数据写入到加密流中 
                cs.Write(YourInputStorage, 0, YourInputStorage.Length);
                //关闭加密流对象 
                cs.FlushFinalBlock();
                //把加密后的数据转换成字符串 
                string strEncrypt = Convert.ToBase64String(ms.ToArray());
                //关闭内存流 
                ms.Close();
                //返回加密后的字符串 
                return strEncrypt;
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        /// <summary> 
        /// 解密 
        /// </summary> 
        public static string DesDecode(string strText)
        {
            byte[] byKey = { 0x12, 0x34, 0x56, 0x78, 0x90, 0x13, 0x57, 0x90 };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0x22, 0x44, 0x66 };
            byte[] inputByteArray = new Byte[strText.Length];
            try
            {
                //创建一个DES算法的解密类 
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //将字符串转换成字节 
                inputByteArray = Convert.FromBase64String(strText);
                //在内存中创建一个支持存储区的流 
                MemoryStream ms = new MemoryStream();
                //CryptoStream对象的作用是将数据流连接到解密转换的流 
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                //将字节数组中的数据写入到解密流中 
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                //关闭加密流对象 
                cs.FlushFinalBlock();
                //把加密后的数据转换成字符串 
                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                string strDecode = encoding.GetString(ms.ToArray());
                //关闭内存流 
                ms.Close();
                //返回加密后的字符串 
                return strDecode;
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        /// <summary>
        /// 读取二进制文件
        /// </summary>
        /// <param name="filename"></param>
        private void ReadBinaryFiles(string filename)
        {
            FileStream filesstream = new FileStream(filename, FileMode.Create);
            BinaryWriter objBinaryWriter = new BinaryWriter(filesstream);
            for (int index = 0; index < 20; index++)
            {
                objBinaryWriter.Write((int)index);
            }
            objBinaryWriter.Close();
            filesstream.Close();
        }

        /// <summary>
        /// 写入二进制文件
        /// </summary>
        /// <param name="filepath"></param>
        private void WriteBinaryFiles(string filepath)
        {
            if (!File.Exists(filepath))
            {
                //文件不存在
            }
            else
            {
                FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                BinaryReader objBinaryReader = new BinaryReader(filestream);
                try
                {
                    while (true)
                    {
                        //objBinaryReader.ReadInt32();
                    }
                }
                catch (Exception ex)
                {
                    //已到文件结尾
                }
            }
        }
    }
}