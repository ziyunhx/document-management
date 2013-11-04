namespace Commons
{
    using System;
    using System.IO;
    using System.Text;

    public class ReaderHTml
    {
        public static string readerString(string path)
        {
            StreamReader reader = new StreamReader(path, Encoding.Default);
            return reader.ReadToEnd();
        }
    }
}

