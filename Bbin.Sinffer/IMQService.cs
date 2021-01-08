using Bbin.Core.Commandargs;
using Bbin.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer
{
    public interface IMQService
    {
        string QueueName { get; }
        /// <summary>
        /// 向 manager 服务发送上线通知
        /// </summary>
        void PublishUp(SnifferUpArgs snifferUpArgs);
        /// <summary>
        /// 将采集到的 round 推送到队列
        /// </summary>
        /// <param name="roundModel"></param>
        void PublishRound(RoundModel roundModel);
        /// <summary>
        /// 侦听 manager 服务广播的命令
        /// </summary>
        void ListenerManager();
    }
}
