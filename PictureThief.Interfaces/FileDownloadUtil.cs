using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace PictureThief.Interfaces
{
    public static class FileDownloadUtil
    {
        public static bool DownloadFile(string hostname, string uri, string saveTo)
        {
            int BUFF_SIZE = 4096;
            byte[] BUFF = new byte[BUFF_SIZE];

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream stream = null;
            FileStream fs = null;

            try
            {
                string requestStr = string.Empty;

                if (IsFulUrl(uri))
                {
                    requestStr = uri;
                }
                else
                {
                    requestStr = DownloadConfig.Protocol + "://" + CombindCorrectUrl(hostname, uri);
                }

                request = WebRequest.Create(requestStr) as HttpWebRequest;

                Logger.Instance.Info("FileDownload: " + DownloadConfig.Protocol + "://" + CombindCorrectUrl(hostname, uri));

                request.Timeout = DownloadConfig.WebTimeout;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.KeepAlive = true;

                response = request.GetResponse() as HttpWebResponse;

                using (stream = response.GetResponseStream())
                {
                    using (fs = new FileStream(saveTo, FileMode.CreateNew, FileAccess.ReadWrite))
                    {

                        string readLine = string.Empty;

                        Logger.Instance.Info("Transfer started at " + DateTime.Now.ToLongTimeString());

                        int bytesRead = 0;
                        while ((bytesRead = stream.Read(BUFF, 0, BUFF_SIZE)) > 0)
                        {
                            fs.Write(BUFF, 0, bytesRead);
                        }

                        fs.Flush();
                    }

                    Logger.Instance.Info("Transfer Finish at " + DateTime.Now.ToLongTimeString());
                    stream.Close();
                }

                response.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);

                return false;
            }
        }

        public static bool DownloadFile(string uri, string saveTo)
        {
            int BUFF_SIZE = 4096;
            byte[] BUFF = new byte[BUFF_SIZE];

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream stream = null;
            FileStream fs = null;

            try
            {

                request = WebRequest.Create(uri) as HttpWebRequest;

                Logger.Instance.Info("FileDownload: " + uri);

                request.Timeout = DownloadConfig.WebTimeout;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.KeepAlive = true;
                response = request.GetResponse() as HttpWebResponse;

                using (stream = response.GetResponseStream())
                {
                    using (fs = new FileStream(saveTo, FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        string readLine = string.Empty;

                        Logger.Instance.Info("Transfer started at " + DateTime.Now.ToLongTimeString());

                        int bytesRead = 0;
                        while ((bytesRead = stream.Read(BUFF, 0, BUFF_SIZE)) > 0)
                        {
                            fs.Write(BUFF, 0, bytesRead);
                        }

                        fs.Flush();
                    }

                    Logger.Instance.Info("Transfer Finish at " + DateTime.Now.ToLongTimeString());
                    stream.Close();
                }

                response.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);

                return false;
            }
        }

        public static bool DownloadFile(string hostname, string uri, ref MemoryStream fileContent, bool useSimData)
        {
            int BUFF_SIZE = 4096;
            byte[] BUFF = new byte[BUFF_SIZE];

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream stream = null;

            try
            {
                string requestStr = string.Empty;

                if (IsFulUrl(uri))
                {
                    requestStr = uri;
                }
                else
                {
                    requestStr = DownloadConfig.Protocol + "://" + CombindCorrectUrl(hostname, uri);
                }

                request = WebRequest.Create(requestStr) as HttpWebRequest;

                Logger.Instance.Info("FileDownload: " + requestStr);

                request.Timeout = DownloadConfig.WebTimeout;
                request.AutomaticDecompression = DecompressionMethods.GZip;

                if (useSimData)
                {
                    request.KeepAlive = true;
                    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8";
                    request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate,sdch";
                    request.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1,utf-8;q=0.7,*;q=0.3";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11";
                }

                response = request.GetResponse() as HttpWebResponse;

                using (stream = response.GetResponseStream())
                {
                    string readLine = string.Empty;

                    Logger.Instance.Info("Transfer started at " + DateTime.Now.ToLongTimeString());

                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(BUFF, 0, BUFF_SIZE)) > 0)
                    {
                        fileContent.Write(BUFF, 0, bytesRead);
                    }

                    Logger.Instance.Info("Transfer Finish at " + DateTime.Now.ToLongTimeString());

                    stream.Close();
                    
                }

                response.Close();

                return true;

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.ToString());

                return false;
            }
        }

        public static bool DownloadFile(string uri, ref MemoryStream fileContent, bool useSimData)
        {
            int BUFF_SIZE = 4096;
            byte[] BUFF = new byte[BUFF_SIZE];

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream stream = null;

            try
            {
                string requestStr = string.Empty;

                requestStr = uri;


                request = WebRequest.Create(requestStr) as HttpWebRequest;

                Logger.Instance.Info("FileDownload: " + requestStr);

                request.Timeout = DownloadConfig.WebTimeout;
                request.AutomaticDecompression = DecompressionMethods.GZip;

                if (useSimData)
                {
                    request.KeepAlive = true;
                    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8";
                    request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate,sdch";
                    request.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1,utf-8;q=0.7,*;q=0.3";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11";
                }

                response = request.GetResponse() as HttpWebResponse;

                using (stream = response.GetResponseStream())
                {
                    string readLine = string.Empty;

                    Logger.Instance.Info("Transfer started at " + DateTime.Now.ToLongTimeString());

                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(BUFF, 0, BUFF_SIZE)) > 0)
                    {
                        fileContent.Write(BUFF, 0, bytesRead);
                    }

                    Logger.Instance.Info("Transfer Finish at " + DateTime.Now.ToLongTimeString());

                    stream.Close();

                }

                response.Close();

                return true;

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.ToString());

                return false;
            }
        }

        public static bool DownloadWebPage(string hostname, string uri, ref string fileContent, ref Encoding encoding)
        {
            fileContent = string.Empty;

            MemoryStream ms = new MemoryStream();

            if (DownloadFile(hostname, uri, ref ms, true))
            {
                fileContent = Encoding.ASCII.GetString(ms.ToArray());

                encoding = GetEncoding(fileContent);

                if (encoding != null)
                {
                    Logger.Instance.Info("Get Encoding OK");
                    ms.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(ms, encoding);

                    fileContent = sr.ReadToEnd();
                    Logger.Instance.Info("Get File Content OK");
                }

                return true;
            }

            return false;
        }

        public static bool DownloadWebPage( string uri, ref string fileContent, ref Encoding encoding)
        {
            fileContent = string.Empty;

            MemoryStream ms = new MemoryStream();

            if (DownloadFile(uri, ref ms, true))
            {
                fileContent = Encoding.ASCII.GetString(ms.ToArray());

                encoding = GetEncoding(fileContent);

                if (encoding != null)
                {
                    Logger.Instance.Info("Get Encoding OK");
                    ms.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(ms, encoding);

                    fileContent = sr.ReadToEnd();
                    Logger.Instance.Info("Get File Content OK");
                }

                return true;
            }

            return false;
        }

        public static Encoding GetEncoding(string html)
        {
            string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";
            string charset = Regex.Match(html, pattern).Groups["charset"].Value;
            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static bool IsFulUrl(string url)
        {
            string urlPattern = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

            Match match = Regex.Match(url, urlPattern);

            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetCorrectUrl(string mainHost, string currentUrl, string fileUrl)
        {
            string urlPattern = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

            Match match = Regex.Match(fileUrl, urlPattern);

            if (match.Success)
            {
                return fileUrl;
            }
            else
            {
                string firstPart = CombindCorrectUrl(mainHost, currentUrl);
                string secondPart = CombindCorrectUrl(firstPart, fileUrl);

                return secondPart;
            }
        }

        public static string GetCorrectCurrentUrl(string currentInput)
        {
            //string fileNameWithExtenstionPattern = @"^[\w0-9&#228;&#196;&#246;&#214;&#252;&#220;&#223;\-_]+\.[a-zA-Z0-9]{2,}$";
            string fileNameWithExtenstionPattern = @"^[\w0-9&#228;&#196;&#246;&#214;&#252;&#220;&#223;\-_]+\..{2,}$";
            
            string currentDir = currentInput;

            int lastSlash = currentInput.LastIndexOf('/');

            if (lastSlash > 0 && lastSlash != currentInput.Length - 1)
            {
                string lastPart = currentInput.Substring(lastSlash + 1);

                Match match = Regex.Match(lastPart, fileNameWithExtenstionPattern);

                if (match.Success)
                {
                    currentDir = currentInput.Substring(0, lastSlash);
                }
            }

            return currentDir;
        }

        public static string CombindCorrectUrl(string partOne, string partTwo)
        {
            while (partOne.EndsWith("/"))
            {
                partOne = partOne.Substring(0, partOne.Length - 1);
            }

            while (partTwo.StartsWith("/"))
            {
                partTwo = partTwo.Substring(1);
            }

            bool addSlash = false;
            while (partTwo.EndsWith("/"))
            {
                partTwo = partTwo.Substring(0, partTwo.Length - 1);
                addSlash = true;
            }

            string combind = string.Format("{0}/{1}", partOne, partTwo);
            if (addSlash)
            {
                combind = string.Format("{0}/", combind);
            }

            return combind;
        }

        public static string GetRootUrl(string hostname, string currentUrl, string url)
        {
            if (url.StartsWith("/"))
            {
                return hostname;
            }
            else
            {
                return currentUrl;
            }
        }

        public static string GetUrlFromHost(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            url = url.TrimStart().TrimEnd();
            string pattern = @"^((http|ftp|https):\/\/)?(([a-zA-Z0-9\-]*\.{1,}){1,}[a-zA-Z0-9]*)";

            Match match = Regex.Match(url, pattern);

            if (match.Success)
            {
                string newUrl = Regex.Replace(url, pattern, "");

                return newUrl;
            }

            return url;
        }

        public static string GetInvalidPath(string str)
        {
            string invalid = new string(System.IO.Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                str = str.Replace(c.ToString(), "");
            }

            return str;
        }
    }
}
