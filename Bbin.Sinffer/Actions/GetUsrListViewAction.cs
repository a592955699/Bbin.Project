using Bbin.Sniffer;
using System.Collections.Generic;

namespace Bbin.Sniffer.Actions
{
    public class GetUsrListViewAction : IActionExecutor
    {
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
        }
        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
