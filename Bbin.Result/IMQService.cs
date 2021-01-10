using Bbin.Core.Model;
using Bbin.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Result
{
    public interface IMQService
    {
        /// <summary>
        /// 将处理的 ResultId 推送到队列
        /// </summary>
        /// <param name="rs">处理的结果Id</param>
        void PublishResult(string rs);
        /// <summary>
        /// 侦听 sniffer 推送过来的 round 命令
        /// </summary>
        void ListenerManager(Action<QueueModel<RoundModel>> action);
    }
}
