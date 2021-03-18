using Baidu.Aip.Ocr;
using Bbin.Core.Configs;
using Bbin.Core.Entitys;
using Bbin.BaiduAI.Config;
using Bbin.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Bbin.Sniffer
{
    public class YongLi2LoginService : AbstractLoginService
    {
        private BaiduConfig baiduConfig;
        public YongLi2LoginService(SiteConfig _siteConfig, BaiduConfig _baiduConfig) : base(_siteConfig)
        {
            baiduConfig = _baiduConfig;
        }

        #region 永利登录、退出、检查登录模块
        public override bool InternalCheckLogin()
        {
            string url = siteConfig.Domain + "index.php/index/N_index";
            string html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("【提示】验证登录页 ,请求 Url:{0}", url);
            log.DebugFormat("【提示】验证登录页 ,响应 Html:\r\n{0}", html);
            bool loginState = html.Contains(siteConfig.UserName);
            log.InfoFormat("【提示】登录状态:{0}", loginState);
            return loginState;
        }

        public override bool InternalLogin()
        {
            if (CookieContainer == null) CookieContainer = new CookieContainer();
            string url = "";
            string data = "";
            string html = "";

            //访问首页
            url = "http://www.800800s.com/?init=index";
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("【提示】请求首页,请求 Url:{0}", url);
            log.DebugFormat("【提示】请求首页,响应 html:\r\n{0}", html);

            //访问框架页
            url = "http://www.800800s.com/y9941tpl/myhome.php";
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("【提示】请求首页 IFrame,请求 Url:{0}", url);
            log.DebugFormat("【提示】请求首页 IFrame,响应 html:\r\n{0}", html);


            #region 获取验证码
            string imageDir = Path.Combine(AppContext.BaseDirectory, "code");// Path.Combine(AppContext.BaseDirectory, "code.png");
            string pngPath = Path.Combine(imageDir, "code.png");

            if (!Directory.Exists(imageDir))
            {
                Directory.CreateDirectory(imageDir);
            }
            url = "http://www.800800s.com/include/vcode.php?bk=FFFFF&space=15&color=000000&mode=middle&name=loginVcode";
            HttpUtil.DownloadFile(url, pngPath, cookieContainer: CookieContainer);

            var client = new Ocr(baiduConfig.ApiKey, baiduConfig.SecretKey);
            client.Timeout = 60000;
            var image = File.ReadAllBytes(pngPath);
            var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GeneralBasic(image, options);
            var words = result.SelectToken("words_result");
            if (!words.Any())
            {
                log.Info("【提示】调用百度接口识别图片异常");
                return false;
            }
            var code = words.FirstOrDefault()["words"].ToString();
            #endregion


            url = "http://www.800800s.com/mobile/controller/check_touch_captcha.php?username=hujunmin0";
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("\r\n访问 " + url);
            log.InfoFormat(html);

            //登录
            url = "http://www.800800s.com/logincheck.php";
            data = "r=" + DateTime.Now.ToFileTime() + "&action=login&vlcodes=" + code + "&username=hujunmin0&password=hujunmin01";
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            if (CookieContainer != null)
                httpWebRequest.CookieContainer = CookieContainer;

            httpWebRequest.Accept = "*/*";
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpWebRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            httpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            httpWebRequest.Headers.Add("DNT", "1");
            httpWebRequest.Headers.Add("Host", "www.800800s.com");
            httpWebRequest.Headers.Add("Host", "www.800800s.com");
            httpWebRequest.Headers.Add("Origin", "http://www.800800s.com");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            httpWebRequest.Referer = "http://www.800800s.com/y9941tpl/myhome.php";

            httpWebRequest.Timeout = 1000 * 30;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.KeepAlive = false;
            html = HttpUtil.DoPostAsycn(httpWebRequest, data, Encoding.UTF8).Result;
            log.InfoFormat("【提示】请求登录,请求 Url:{0}", url);
            log.DebugFormat("【提示】请求登录,响应 html:\r\n{0}", html);

            //验证规则
            url = "http://www.800800s.com/y9941tpl/chk_rule.php";
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("【提示】请求同意协议,请求 Url:{0}", url);
            log.DebugFormat("【提示】请求同意协议,响应 html:\r\n{0}", html);

            url = "http://www.800800s.com/y9941tpl/myhome.php";
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("【提示】请求首页,请求 Url:{0}", url);
            log.DebugFormat("【提示】请求首页,响应 html:\r\n{0}", html);
            var login = html.Contains("hujunmin0");
            log.InfoFormat("【提示】登录状态:{0}", login);
            return login;
        }

        public override void InternalLogout()
        {
            log.Info("模拟退出登录");
        } 
        #endregion

        public override void InternalGetBbinSessionId()
        {
            #region 
            var url = "http://www.800800s.com/agline/index.php?type=BBIN";
            log.InfoFormat("【提示】bbin跳转页 ,请求 Url:{0}", url);
            var html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.DebugFormat("【提示】bbin跳转页 响应 Html:\r\n{0}", url);
            #endregion

            #region bbin
            string partten = "<script>window.location.replace\\('(.*?)'\\)</script>";
            log.DebugFormat("【提示】bbin跳转页 正则:{0}", partten);
            Regex reg = new Regex(partten, GetRegexOptions());
            var match = reg.Match(html);
            if (match.Success)
            {
                url = match.Groups[1].Value;
                log.InfoFormat("【提示】请求bbin ,Url:{0}", url);
                html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
                log.DebugFormat("【提示】请求bbin, 响应 Html:\r\n{0}", html);

                var cookies = CookieContainer.GetCookies(new Uri(url));
                foreach (Cookie item in cookies)
                {
                    if (item.Name == "SESSION_ID")
                    {
                        SessionId = item.Value;
                        break;
                    }
                }

                if (!String.IsNullOrWhiteSpace(SessionId))
                {
                    log.InfoFormat("【提示】恭喜您，拿到 bbin sid:{0}", SessionId);
                }
                else
                {
                    log.Warn("【警告】抱歉，破解 bbin 登录失败");
                }
            }
            else
            {
                log.Warn("【警告】bbin跳转页 响应结果无法匹配到响应的数据");
            }
            #endregion
        }

        /// <summary>
        /// 正则匹配项设置
        /// </summary>
        /// <returns></returns>
        private RegexOptions GetRegexOptions()
        {
            RegexOptions objOptions = RegexOptions.Singleline;
            objOptions |= RegexOptions.IgnoreCase;
            return objOptions;
        }
    }
}
