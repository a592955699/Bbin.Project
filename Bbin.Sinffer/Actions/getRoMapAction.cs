using Bbin.Core.Cons;
using Bbin.Sniffer;
using Bbin.Sniffer.Cons;
using log4net;
using System.Collections.Generic;

namespace Bbin.Sniffer.Actions
{
    public class getRoMapAction : IInternalActionExecutor
    {
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(getRoMapAction));
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
            object runError = string.Empty;
            if (data.TryGetValue("runEor", out runError))
            {
                if (runError.ToString() == "IDLE_10M")
                {
                    log.Info("【提示】"+WebSocketColseCodes.ActivityIDLE_10M);
                }
            }
        }

        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
