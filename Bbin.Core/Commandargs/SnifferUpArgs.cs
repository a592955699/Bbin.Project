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
        public string QueueName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastDateTime { get; set; } = DateTime.Now;
    }
}
