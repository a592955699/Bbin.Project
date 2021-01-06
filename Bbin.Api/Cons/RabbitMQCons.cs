using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Api.Cons
{
    public class RabbitMQCons
    {
        /// <summary>
        /// sniffer 服务推送 round 的 队列Key
        /// </summary>
        public static readonly string RoundQueue = "BBIN.QUQUE.ROUND";
        ///// <summary>
        ///// 
        ///// </summary>
        //public static readonly string ResultQueue = "BBIN.QUQUE.RESULT";
        /// <summary>
        /// manager 接收消息的队列 key
        /// </summary>
        public static readonly string ManagerQueue = "BBIN.QUQUE.RESULT";
        /// <summary>
        /// manager 服务广播交换机 key
        /// </summary>
        public static readonly string ManagerExchange = "BBIN.EXCHANGE.MANAGER";
    }
}
