using Bbin.Sniffer;
using Bbin.Sniffer.SnifferActionExecutors;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bbin.SnifferActionExecutors
{
    public class PingAction : AbstractWsActionExecutor
    {
        public override void Execute(params object[] paras)
        {
            var jsonString = JsonConvert.SerializeObject(Data);
            SocketService.Send(jsonString);
        }
    }
}
