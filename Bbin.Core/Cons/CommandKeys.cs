using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Cons
{
    public class CommandKeys
    {
        /// <summary>
        /// sniffer 服务向 result服务发送 round 结果命令
        /// </summary>
        public static string PublishRound = "PublishRound";
        /// <summary>
        /// sniffer 服务向 manager 服务发送上线通知
        /// </summary>
        public static string PublishSnifferUp = "PublishSnifferUp";
        /// <summary>
        /// result 服务向 manager 服务发送 result 结果命令
        /// </summary>
        public static string PublishResult = "PublishResult";
        /// <summary>
        /// manger 服务向 sniffer 发送 start 命令
        /// </summary>
        public static string PublishSnifferStart = "ManagerPublishStart";
        /// <summary>
        /// manger 服务向 sniffer 发送 stop 命令
        /// </summary>
        public static string PublishSnifferStop = "ManagerPublishStop";
    }
}
