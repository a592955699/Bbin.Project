using Bbin.BaiduAI.ImageOrc.Model;
using Bbin.Core.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Bbin.BaiduAI.ImageOrc
{
    public class AdvancedGeneral
    {
        /// <summary>
        /// 通用物体和场景识别
        /// </summary>
        /// <param name="token">调用鉴权接口获取的token</param>
        /// <param name="localFilePath">本地图片路径</param>
        /// <returns></returns>
        public static AdvancedGeneralResult advancedGeneral(string token, string localFilePath)
        {
            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v2/advanced_general?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = FileUtil.FileToBase64(localFilePath);
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string result = reader.ReadToEnd();
            //Console.WriteLine("通用物体和场景识别:");
            //Console.WriteLine(result);
            return JsonConvert.DeserializeObject<AdvancedGeneralResult>(result);
        }

        /// <summary>
        /// 通用物体和场景识别并判断是否包括指定 keyword
        /// </summary>
        /// <param name="token">调用鉴权接口获取的token</param>
        /// <param name="localFilePath">本地图片路径</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public static bool advancedGeneral(string token, string localFilePath, string keyword)
        {
            var charArray = keyword.ToArray();
            var advancedGeneralResult = AdvancedGeneral.advancedGeneral(token, localFilePath);
            return advancedGeneralResult.result.Any(x => { return x.keyword.ToArray().Intersect(charArray).Any() || x.root.ToArray().Intersect(charArray).Any(); });
        }
    }
}
