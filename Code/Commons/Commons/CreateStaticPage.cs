namespace Commons
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class CreateStaticPage
    {
        public static void GetResponseText(string url, string fileName)
        {
            string str = null;
            Stream responseStream = null;
            StreamReader reader = null;
            FileStream stream2 = null;
            StreamWriter writer = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (response.StatusDescription == "OK")
                {
                    try
                    {
                        responseStream = response.GetResponseStream();
                        reader = new StreamReader(responseStream, Encoding.UTF8);
                        str = reader.ReadToEnd();
                        stream2 = new FileStream(fileName, FileMode.Create);
                        writer = new StreamWriter(stream2, Encoding.UTF8);
                        writer.Write(str);
                    }
                    finally
                    {
                        writer.Close();
                        stream2.Close();
                        reader.Close();
                        responseStream.Close();
                    }
                }
                response.Close();
            }
            catch
            {
            }
        }

        public static void getText(string str, string tpath)
        {
            FileStream stream = null;
            StreamWriter writer = null;
            try
            {
                stream = new FileStream(tpath, FileMode.Create);
                writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(str);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    stream.Close();
                }
            }
        }

        public static void ReaderText(string str, string tpath)
        {
            FileStream stream = null;
            StreamWriter writer = null;
            try
            {
                stream = new FileStream(tpath, FileMode.Append);
                writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(str);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    stream.Close();
                }
            }
        }
    }
}

