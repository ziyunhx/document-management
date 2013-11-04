namespace Commons
{
    using System;
    using System.Threading;

    public class CreateFile
    {
        public static void CreateHtmlFile(string URL, string PATH)
        {
            string str2;
            string webResponse = BUtility.GetWebResponse(URL);
            if (webResponse == "")
            {
                return;
            }
            int num = 0;
        Label_0017:
            str2 = PATH;
            if ((FileUtil.FileExists(str2) || (FileUtil.FileCreate(str2, 1) == 1)) && !FileUtil.WriteFileContent(str2, webResponse, "UTF-8", false))
            {
                num++;
                if (num < 4)
                {
                    Thread.Sleep(100);
                    goto Label_0017;
                }
            }
        }
    }
}

