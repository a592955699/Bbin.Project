using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.BaiduAI.ImageOrc.Model
{
    /// <summary>
    /// 百度图像识别 AccessToken 返回结果model
    /// </summary>
    public class AccessTokenResult
    {
        public string refresh_token { get; set; }
        public long expires_in { get; set; }
        public string session_key { get; set; }
        public string access_token { get; set; }
        public string scope { get; set; }
        public string session_secret { get; set; }
    }
}
