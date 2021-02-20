using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Configs
{
    public class SiteConfig
    {
        /// <summary>
        /// 主域名,请以 ‘/’ 结尾
        /// </summary>
        public string Domain = "http://www.800800s.com/";
        public string UserName { get; set; } = "hujunmin0";
        public string PassWord { get; set; } = "hujunmin01";
        /// <summary>
        /// 验证码重试次数
        /// </summary>
        public int CheckCodeTryTimes = 50;
    }
}
