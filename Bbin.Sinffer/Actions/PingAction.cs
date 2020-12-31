using Bbin.Sniffer;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bbin.Sniffer.Actions
{
    public class PingAction : IActionExecutor
    {
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            webSocketWrap.Send(jsonString);
        }
        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
