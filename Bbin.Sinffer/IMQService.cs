using Bbin.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer
{
    public interface IMQService
    {
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
