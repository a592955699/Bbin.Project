using Bbin.Core.Entitys;
using Bbin.Core.Cons;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Bbin.Core.Configs;

namespace Bbin.Sniffer
{
    public abstract class AbstractLoginService 
    {
        public AbstractLoginService(SiteConfig _siteConfig)
        {
            siteConfig = _siteConfig;
        }

        string cookieFileName = "cookie.txt";
        protected static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(AbstractLoginService));
        public CookieContainer CookieContainer { get; private set; }
        public SiteConfig siteConfig;
        public string SessionId { get; protected set; }
        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <returns></returns>
        public bool CheckLogin()
        {
            //第一次，默认先去加载缓存中的 CookieContainer
            if (CookieContainer == null)
            {
                CookieContainer = new CookieContainer();
                log.Debug("【提示】尝试去缓存中获取 CookieContainer ");
                LoadCookieContainer();
            }

            //已经登录，则直接返回
            if (InternalCheckLogin())
            {
                log.Debug("【提示】内部登录检查通过");
                return true;
            }

            //未登录，则执行登录
            return false;
        }

        public bool CheckAndLogin()
        {
            if (CheckLogin())
            {
                InternalGetSessionId();
                return true;
            }

            var loginState = Login();
            if (!loginState)
                return false;
            InternalGetSessionId();
            return true;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            //未登录，则执行登录
            bool loginState = InternalLogin();
            if (loginState)
            {
                log.Debug("【提示】内部重新登录，开始写 CookieContainer 缓存");
                SaveCookieContainer();
            }
            else
            {
                log.Warn("【警告】内部登录失败");
            }
            return loginState;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public void Logout()
        {
            log.Debug("【提示】准备退出登录");
            InternalLogout();
        }

        /// <summary>
        /// 去缓存中加载 cookie 信息
        /// </summary>
        public virtual void LoadCookieContainer()
        {
            try
            {
                if (File.Exists(cookieFileName))
                {
                    var uri = new Uri(siteConfig.Domain);
                    using (StreamReader sr = new StreamReader(cookieFileName, Encoding.UTF8))
                    {
                        var json = sr.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(json))
                            return;
                        var cookies = JsonConvert.DeserializeObject<List<Cookie>>(json);
                        if (cookies == null || cookies.Count <= 0) return;

                        //加入到 CookieContainer
                        foreach (Cookie item in cookies)
                        {
                            var newCookie = new Cookie(item.Name, item.Value, item.Path, item.Domain);
                            if (item.Expires != DateTime.MinValue)
                                newCookie.Expires = item.Expires;
                            CookieContainer.Add(uri, newCookie);
                            log.DebugFormat("【提示】从文件中加载 Cookie:{0}", JsonConvert.SerializeObject(newCookie));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("【错误】从文件中加载 Cookie 异常，Path:{0}", cookieFileName, ex);
            }
        }
        /// <summary>
        /// 将 CookieContainer 写入缓存
        /// </summary>
        public virtual void SaveCookieContainer()
        {
            try
            {
                var uri = new Uri(siteConfig.Domain);
                var cookieCollection = CookieContainer.GetCookies(uri);
                if (cookieCollection.Count > 0)
                {
                    var cookies = new List<Cookie>();
                    foreach (Cookie item in cookieCollection)
                    {
                        cookies.Add(item);
                    }
                    var jsonStr = JsonConvert.SerializeObject(cookies);
                    using (FileStream fs = new FileStream(cookieFileName, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter sr = new StreamWriter(fs, Encoding.UTF8))
                        {
                            sr.Write(jsonStr);
                            sr.Flush();
                            sr.Close();
                            sr.Dispose();
                        }
                        fs.Dispose();
                    }
                    log.DebugFormat("【提示】写入 Cookies 缓存:{0}", JsonConvert.SerializeObject(cookies));
                }
            }
            catch (Exception ex)
            {
                log.Error("【错误】写入 Cookie 缓存失败！", ex);
            }
        }
        /// <summary>
        /// 具体判断登录逻辑
        /// </summary>
        /// <returns></returns>
        public abstract bool InternalCheckLogin();
        /// <summary>
        /// 具体登录逻辑
        /// 登录后，需要给 SessionId 赋值
        /// </summary>
        /// <returns></returns>
        public abstract bool InternalLogin();
        /// <summary>
        /// 具体获取 SessionId 逻辑
        /// </summary>
        public abstract void InternalGetSessionId();
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public abstract void InternalLogout();
    }
}
