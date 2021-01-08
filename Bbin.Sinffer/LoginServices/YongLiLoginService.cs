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

namespace Bbin.Sniffer
{
    public class YongLiLoginService : AbstractLoginService
    {
        private BaiduConfig baiduConfig;
        public YongLiLoginService(SiteConfig _siteConfig, BaiduConfig _baiduConfig) : base(_siteConfig)
        {
            baiduConfig = _baiduConfig;
        }

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

        public override void InternalGetSessionId()
        {
            #region 
            var url = siteConfig.Domain + "index.php/video/login?g_type=bbin";
            log.InfoFormat("【提示】bbin跳转页 ,请求 Url:{0}", url);
            var html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.DebugFormat("【提示】bbin跳转页 响应 Html:\r\n{0}", url);
            #endregion

            #region bbin
            string partten = "<script type='text/javascript'>[\\s]+window.document.location=\"(.*?)\";[\\s]+</script>";
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

        public override bool InternalLogin()
        {
            string imageDir = Path.Combine(AppContext.BaseDirectory, "code");// Path.Combine(AppContext.BaseDirectory, "code.png");

            string pngPath = Path.Combine(imageDir, "code.png");


            if (!Directory.Exists(imageDir))
            {
                log.DebugFormat("【提示】图片目录不存在，准备创建目录:{0}", imageDir);
                Directory.CreateDirectory(imageDir);
            }

            //string sessonId="";
            string url = "";
            string data = "";
            string html = "";
            string code = "";

            #region 访问首页，获取 CookieContainer
            url = siteConfig.Domain;
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.InfoFormat("【提示】请求首页,请求 Url:{0}", url);
            log.DebugFormat("【提示】请求首页,响应 html:\r\n{0}", html);
            #endregion


            #region 获取验证码
            url = siteConfig.Domain + "yzm.php?type=" + DateTime.Now.ToFileTime();
            log.InfoFormat("【提示】请求验证码,请求 Url:{0} 证码图片地址:{1}", url, pngPath);
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
                log.InfoFormat("【提示】破解验证码失败,图片地址:{0} ", pngPath);
                return false;
            }
            code = words.FirstOrDefault()["words"].ToString();
            #endregion

            #region 
            url = siteConfig.Domain + "index.php/webcenter/Login/login_do";
            //data = "r=" + DateTime.Now.ToFileTime() + "&action=login&username=" + siteConfig.UserName + "&password=" + siteConfig.PassWrod + "&vlcodes=null";
            data = $"r={DateTime.Now.ToFileTime()}&action=login&username={siteConfig.UserName}&password={siteConfig.PassWrod}&vlcodes={code}";
            log.InfoFormat("【提示】请求 login_do ,请求 Url:{0} Data:{1}", url, data);
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            if (CookieContainer != null)
                httpWebRequest.CookieContainer = CookieContainer;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            httpWebRequest.Timeout = 1000 * 30;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.Accept = "*/*";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.KeepAlive = false;
            html = HttpUtil.DoPostAsycn(httpWebRequest, data, Encoding.UTF8).Result;
            log.DebugFormat("【提示】请求 login_do ,响应Html:\r\n{0} ", html);

            if (html != "1" && html != "10" && html != "5")
            {
                log.WarnFormat("【警告】请求 login_do, 破解失败,正常值:1,现状值:{0}", html);
                return false;
            }
            #endregion

            #region 
            url = siteConfig.Domain + "index.php/webcenter/Login/login_info";
            log.InfoFormat("【提示】请求 login_info ,请求 Url:{0}", url);
            html = HttpUtil.DoGetAsycn(url, CookieContainer).Result;
            log.DebugFormat("【提示】请求 login_info, 响应 Html:\r\n{0}", html);
            #endregion

            #region 
            url = siteConfig.Domain + "index.php/Index/N_index";
            data = "submit=我同意";
            log.InfoFormat("【提示】请求协议页 ,请求 Url:{0} Data={1}", url, data);
            html = HttpUtil.DoPostAsycn(url, data, CookieContainer).Result;
            log.DebugFormat("【提示】请求协议页 ,响应 Html:\r\n{0}", html);

            bool login = html.Contains(siteConfig.UserName);
            log.InfoFormat("【提示】登录状态:{0}", login);
            return login;
            #endregion
        }

        public override void InternalLogout()
        {
            log.Info("模拟退出登录");
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
