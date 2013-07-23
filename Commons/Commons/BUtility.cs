namespace Commons
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class BUtility
    {
        public static Stream GetStream(string URL)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);
                response = (HttpWebResponse) request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

        public static string GetWebResponse(string URL)
        {
            string str = "";
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));
            str = reader.ReadToEnd();
            response.Close();
            reader.Close();
            request.Abort();
            return str;
        }

        public static string GetWebResponse(string URL, string postData)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);
            request.Method = "POST";
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            Stream requestStream = null;
            try
            {
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                requestStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader reader = null;
            string str = "";
            try
            {
                reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));
                str = reader.ReadToEnd();
            }
            catch (Exception exception2)
            {
                throw exception2;
            }
            finally
            {
                reader.Close();
            }
            return str;
        }

        public static void GetYeePayResponse(string URL, string postData)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);
            request.Method = "POST";
            byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            Stream requestStream = null;
            try
            {
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                requestStream.Close();
            }
        }

        public static string ReadTxtFile(string sPath)
        {
            string str = "";
            StreamReader reader = new StreamReader(sPath, Encoding.GetEncoding("GB2312"));
            try
            {
                str = reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                throw new ApplicationException(exception.Message);
            }
            finally
            {
                reader.Close();
            }
            return str;
        }
    }
}

