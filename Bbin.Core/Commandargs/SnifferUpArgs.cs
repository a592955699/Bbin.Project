using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Commandargs
{
    public class SnifferUpArgs
    {
        /// <summary>
        /// 随机Id，和队列名匹配
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
