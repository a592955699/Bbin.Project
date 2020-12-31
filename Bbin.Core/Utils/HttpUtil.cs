using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bbin.Core.Utils
{
    public class HttpUtil
    {
        public static async Task<string> DoGetAsycn(String url)
        {
            return await DoGetAsycn(url, Encoding.UTF8);
        }
        public static async Task<string> DoGetAsycn(String url, Encoding encoding)
        {
            return await DoGetAsycn(url, encoding, null);
        }
        public static async Task<string> DoGetAsycn(string url, CookieContainer cookieContainer)
        {
            return await DoGetAsycn(url, Encoding.UTF8, cookieContainer);
        }
        public static async Task<string> DoGetAsycn(string url, Encoding encoding, CookieContainer cookieContainer)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            if (cookieContainer != null)
                httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = "text/html";
            httpWebRequest.Timeout = 1000 * 30;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
            //httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.KeepAlive = false;
            return await DoGetAsycn(httpWebRequest, encoding);
        }
        public static async Task<string> DoGetAsycn(HttpWebRequest httpWebRequest, Encoding encoding)
        {
            httpWebRequest.Method = "GET";
            string result = string.Empty;

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    //处理gzip格式的流
                    if (httpWebResponse.ContentEncoding != null && httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        using (StreamReader streamReader = new StreamReader(gzipStream, encoding))
                        {
                            result = await streamReader.ReadToEndAsync();
                            //streamReader.Close();
                        }
                    }
                    else
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, encoding))
                        {
                            result = await streamReader.ReadToEndAsync();
                            //streamReader.Close();
                        }
                    }
                    //responseStream.Close();
                    return result;
                }
            }

        }

        public static async Task<string> DoPostAsycn(String url, CookieContainer cookieContainer)
        {
            return await DoPostAsycn(url, null, Encoding.UTF8, cookieContainer);
        }
        public static async Task<string> DoPostAsycn(String url, String postData, CookieContainer cookieContainer)
        {
            return await DoPostAsycn(url, postData, Encoding.UTF8, cookieContainer);
        }
        public static async Task<string> DoPostAsycn(String url, String postData)
        {
            return await DoPostAsycn(url, postData, Encoding.UTF8, null);
        }
        public static async Task<string> DoPostAsycn(String url, String postData, Encoding encoding)
        {
            return await DoPostAsycn(url, postData, encoding, null);
        }
        public static async Task<string> DoPostAsycn(String url, String postData, Encoding encoding, CookieContainer cookieContainer)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            if (cookieContainer != null)
                httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = "text/html";
            httpWebRequest.Timeout = 1000 * 30;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.KeepAlive = false;
            return await DoPostAsycn(httpWebRequest, postData, encoding);
        }
        public static async Task<string> DoPostAsycn(HttpWebRequest httpWebRequest, String postData, Encoding encoding)
        {
            httpWebRequest.Method = "POST";

            if (!String.IsNullOrWhiteSpace(postData))
            {
                byte[] byteArray = encoding.GetBytes(postData);
                httpWebRequest.ContentLength = byteArray.Length;
                using (Stream reqStream = httpWebRequest.GetRequestStream())
                {
                    await reqStream.WriteAsync(byteArray, 0, byteArray.Length);
                }
            }
            string result = string.Empty;
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    //处理gzip格式的流
                    if (httpWebResponse.ContentEncoding != null && httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        using (StreamReader streamReader = new StreamReader(gzipStream, encoding))
                        {
                            result = await streamReader.ReadToEndAsync();
                            //streamReader.Close();
                        }
                    }
                    else
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, encoding))
                        {
                            result = streamReader.ReadToEnd();
                            //streamReader.Close();
                        }
                    }
                    //responseStream.Close();
                    return result;
                }
            }
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url">所下载的路径</param>
        /// <param name="path">本地保存的路径</param>
        /// <param name="overwrite">当本地路径存在同名文件时是否覆盖</param>
        /// <param name="callback">实时状态回掉函数</param>
        /// Action<文件名,文件的二进制, 文件大小, 当前已上传大小>
        public static void DownloadFile(string url, string path, CookieContainer cookieContainer = null, bool overwrite = true, Action<string, string, byte[], long, long> callback = null)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (cookieContainer != null)
                request.CookieContainer = cookieContainer;

            request.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";

            request.Credentials = CredentialCache.DefaultCredentials;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;


            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //获取文件名
            string fileName = response.Headers["Content-Disposition"];//attachment;filename=FileName.txt
            string contentType = response.Headers["Content-Type"];//attachment;

            if (string.IsNullOrEmpty(fileName))
                fileName = response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1];
            else
                fileName = fileName.Remove(0, fileName.IndexOf("filename=") + 9);
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            using (Stream responseStream = response.GetResponseStream())
            {
                long totalLength = response.ContentLength;
                ///文件byte形式
                byte[] b = Encoding.Default.GetBytes(url);
                //创建本地文件写入流
                //if (System.IO.File.Exists(Path.Combine(path, fileName)))
                //{
                //    fileName = DateTime.Now.Ticks + fileName;
                //}
                using (Stream stream = new FileStream(path, overwrite ? FileMode.Create : FileMode.CreateNew))
                {
                    byte[] bArr = new byte[1024];
                    int size;
                    while ((size = responseStream.Read(bArr, 0, bArr.Length)) > 0)
                    {
                        stream.Write(bArr, 0, size);
                        callback?.Invoke(fileName, contentType, b, totalLength, stream.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 将cookie字符串添加到 CookieContainer
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="cc"></param>
        /// <param name="domian"></param>
        /// <returns></returns>
        public static CookieContainer AddCookieToContainer(string cookie, CookieContainer cc, string domian)
        {
            string[] tempCookies = cookie.Split(';');
            string tempCookie = null;
            int Equallength = 0;//  =的位置
            string cookieKey = null;
            string cookieValue = null;
            //qg.gome.com.cn  cookie
            for (int i = 0; i < tempCookies.Length; i++)
            {
                if (!string.IsNullOrEmpty(tempCookies[i]))
                {
                    tempCookie = tempCookies[i];
                    Equallength = tempCookie.IndexOf("=");

                    if (Equallength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=;
                    {
                        cookieKey = tempCookie.Substring(0, Equallength).Trim();
                        //cookie=

                        if (Equallength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=;
                        {
                            cookieValue = "";
                        }
                        else
                        {
                            cookieValue = tempCookie.Substring(Equallength + 1, tempCookie.Length - Equallength - 1).Trim();
                        }
                    }
                    else
                    {
                        cookieKey = tempCookie.Trim();
                        cookieValue = "";
                    }
                    if (cookieValue.IndexOf(",") != -1)
                    {
                        cookieValue = cookieValue.Replace(",", "%2c");
                    }
                    cc.Add(new Cookie(cookieKey, cookieValue, "", domian));
                }
            }
            return cc;
        }

        /// <summary>
        /// 将相对路径转换成绝对路径
        /// </summary>
        /// <param name="relUrl">相对路径</param>
        /// <param name="pageUrl">相对页路径</param>
        /// <returns></returns>
        public static string GetAbsUrl(string relUrl, string pageUrl)
        {
            string absUrl = string.Empty;
            string[] arrPageUrl;
            string urlPre = string.Empty;

            if (Regex.Matches(pageUrl, "/").Count <= 2)
            {
                pageUrl += "/";
            }
            else if (pageUrl.Substring(pageUrl.LastIndexOf("/")).IndexOf(".") > -1 || pageUrl.IndexOf("/?") > -1 || pageUrl.IndexOf("/#") > -1)
            {
                pageUrl = pageUrl.Substring(0, pageUrl.LastIndexOf("/") + 1);
            }
            else
            {
                if (!pageUrl.EndsWith("/"))
                {
                    pageUrl += "/";
                }
            }

            int urlPrePos = pageUrl.IndexOf("://");

            if (urlPrePos > -1)
            {
                urlPre = pageUrl.Substring(0, urlPrePos + 3);
                pageUrl = pageUrl.Substring(urlPrePos + 3);
            }

            arrPageUrl = pageUrl.Split("/".ToCharArray());

            if (relUrl.IndexOf("://") > -1)//如果是以 "http://"、"ftp://" 等开头的路径
            {
                return relUrl;
            }

            if (relUrl.StartsWith("/"))//如果以 "/" 开头的路径
            {
                return urlPre + arrPageUrl[0] + "/" + relUrl.Substring(1);
            }

            if (relUrl.StartsWith("./"))
            {
                return urlPre + pageUrl + relUrl.Substring(2);
            }

            if (relUrl.StartsWith("../"))
            {
                int level = arrPageUrl.Length - relUrl.Substring(0, relUrl.LastIndexOf("../") + 3).Split("/".ToCharArray()).Length - 1;
                if (level < 0) level = 0;
                for (int i = 0; i <= level; i++)
                {
                    if (absUrl == string.Empty)
                    {
                        absUrl = arrPageUrl[i];
                    }
                    else
                    {
                        absUrl += "/" + arrPageUrl[i];
                    }
                }
                return urlPre + absUrl + relUrl.Substring(relUrl.LastIndexOf("..") + 2);
            }

            return urlPre + pageUrl + relUrl;
        }
    }
}
