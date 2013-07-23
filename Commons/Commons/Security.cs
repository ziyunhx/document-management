namespace Commons
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Security;

    public static class Security
    {
        private static byte[] DESIV = new byte[] { 0x12, 0x34, 0x56, 120, 0x90, 0xab, 0xcd, 0xef };

        public static string CreateAffirmCode()
        {
            Random random = new Random();
            return random.Next(0x186a0, 0xf423f).ToString();
        }

        public static string CreateAffirmCode(int Pos)
        {
            string str = "";
            Random random = new Random();
            for (int i = 0; i < Pos; i++)
            {
                str = str + random.Next(0, 9).ToString();
            }
            return str.ToString();
        }

        public static string Decrypt(string decryptString, string decryptKey)
        {
            if (decryptKey.Trim().Length != 8)
            {
                decryptKey = "87654321";
            }
            try
            {
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(decryptKey);
                byte[] dESIV = DESIV;
                byte[] buffer = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(bytes, dESIV), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        public static string Encrypt(string encriptString, string encKey)
        {
            if (encKey.Trim().Length != 8)
            {
                encKey = "87654321";
            }
            try
            {
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(encKey.Substring(0, 8));
                byte[] dESIV = DESIV;
                byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(encriptString);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(bytes, dESIV), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                return Convert.ToBase64String(stream.ToArray());
            }
            catch
            {
                return encriptString;
            }
        }

        public static string MD5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }
    }
}

