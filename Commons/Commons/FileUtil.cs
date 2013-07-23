namespace Commons
{
    using System;
    using System.IO;
    using System.Text;

    public class FileUtil
    {
        public static bool AppendFileContent(string strSourceFile, string sContent)
        {
            return AppendFileContent(strSourceFile, sContent, "GB2312");
        }

        public static bool AppendFileContent(string strSourceFile, string sContent, string strEncode)
        {
            return WriteFileContent(strSourceFile, sContent, strEncode, true);
        }

        public static bool CopyFile(string strSourceFile, string strTargetFile)
        {
            try
            {
                File.Copy(strSourceFile, strTargetFile, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteFile(string strSourceFileFullPath)
        {
            try
            {
                File.Delete(strSourceFileFullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int DictoryCreate(string strFilePath)
        {
            strFilePath = GetDealedFilePath(strFilePath);
            bool flag = Directory.Exists(strFilePath);
            try
            {
                if (!flag)
                {
                    Directory.CreateDirectory(strFilePath);
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int FileCreate(string strFileAllName, int iSwitch)
        {
            if (DictoryCreate(GetDealedFilePath(GetFileDictory(strFileAllName))) == 0)
            {
                return -2;
            }
            bool flag = File.Exists(strFileAllName);
            if ((iSwitch != 1) && flag)
            {
                return -1;
            }
            try
            {
                if (flag)
                {
                    File.Delete(strFileAllName);
                }
                File.Create(strFileAllName).Close();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static bool FileExists(string strFileAllName)
        {
            return File.Exists(strFileAllName);
        }

        public static string GetDealedFilePath(string strFilePath)
        {
            strFilePath = strFilePath.Trim();
            if (strFilePath.Length == 0)
            {
                return "";
            }
            strFilePath = strFilePath.Replace("/", @"\");
            return strFilePath;
        }

        public static string GetFileContent(string strSourceFile)
        {
            return GetFileContent(strSourceFile, "GB2312");
        }

        public static string GetFileContent(string strSourceFile, string strEnCode)
        {
            string str = "";
            try
            {
                StreamReader reader = new StreamReader(strSourceFile, Encoding.GetEncoding(strEnCode));
                str = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string GetFileDictory(string strFileAllName)
        {
            FileInfo info = new FileInfo(strFileAllName);
            return info.DirectoryName;
        }

        public static string GetFileExtName(string strFileAllName)
        {
            FileInfo info = new FileInfo(strFileAllName);
            return info.Extension;
        }

        public static string GetFileName(string strFileAllName)
        {
            FileInfo info = new FileInfo(strFileAllName);
            return info.Name;
        }

        public static bool ReplaceFileContent(string strSourceFile, string strSourceString, string strTargetString)
        {
            return ReplaceFileContent(strSourceFile, strSourceString, strTargetString, "GB2312");
        }

        public static bool ReplaceFileContent(string strSourceFile, string strSourceString, string strTargetString, string strEncode)
        {
            string sContent = GetFileContent(strSourceFile, strEncode).Replace(strSourceString, strTargetString);
            return WriteFileContent(strSourceFile, sContent, strEncode);
        }

        public static bool WriteFileContent(string strSourceFile, string sContent)
        {
            return WriteFileContent(strSourceFile, sContent, "GB2312");
        }

        public static bool WriteFileContent(string strSourceFile, string sContent, string strEncode)
        {
            return WriteFileContent(strSourceFile, sContent, strEncode, false);
        }

        public static bool WriteFileContent(string strSourceFile, string sContent, string strEncode, bool AppendOrNot)
        {
            try
            {
                FileInfo info = new FileInfo(strSourceFile);
                if (!info.Exists)
                {
                    FileCreate(strSourceFile, 0);
                }
                info = null;
                StreamWriter writer = new StreamWriter(strSourceFile, AppendOrNot, Encoding.GetEncoding(strEncode));
                writer.Write(sContent);
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

