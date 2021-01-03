using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Api.Baccarat.Entitys
{
    public class SiteConfig
    {
        /// <summary>
        /// 主域名,请以 ‘/’ 结尾
        /// </summary>
        public string Domain = "https://www.8876835.com/";
        public string UserName { get; set; } = "hujunmin";
        public string PassWrod { get; set; } = "a123456";
        /// <summary>
        /// 验证码重试次数
        /// </summary>
        public int CheckCodeTryTimes = 50;
    }
}
