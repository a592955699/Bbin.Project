using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Cons
{
    public class RabbitMQCons
    {
        /// <summary>
        /// sniffer 采集到 round
        /// </summary>
        public static readonly string SnifferRoundQueue = "BBIN.ROUND";
        /// <summary>
        /// 根据sniffer 采集的的 round 处理成 result 后
        /// </summary>
        public static readonly string SnifferResuleQueue = "BBIN.RESULT";
    }
}
