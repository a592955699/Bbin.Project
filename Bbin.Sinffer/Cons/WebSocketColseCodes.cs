using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer.Cons
{
    public class WebSocketColseCodes
    {
        /// <summary>
        /// 手动关闭连接
        /// </summary>
        public const ushort ManualShutdown = 3000;
        /// <summary>
        /// SessionId 无效
        /// </summary>
        public const ushort API_EC_ACC_SID_INVALID = 3001;
        /// <summary>
        /// 传输内容超过10M
        /// </summary>
        public const ushort ActivityIDLE_10M = 3002;
        /// <summary>
        /// 传输内容超过5M
        /// </summary>
        public const ushort ActivityIDLE_5M = 3003;
    }
}
