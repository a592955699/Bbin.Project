using Bbin.Sniffer;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bbin.SnifferInternalActionExecutors
{
    public class PingAction : IInternalActionExecutor
    {
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            webSocketWrap.SendAsync(jsonString, (x) => { });
        }
        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
