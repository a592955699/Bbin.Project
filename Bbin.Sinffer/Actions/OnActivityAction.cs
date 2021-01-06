
using Bbin.Api.Cons;
using Bbin.Sniffer;
using log4net;
using System.Collections.Generic;

namespace Bbin.Sniffer.Actions
{
    public class OnActivityAction : IInternalActionExecutor
    {
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(OnActivityAction));
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
            object runError = string.Empty;
            if (data.TryGetValue("runEor", out runError))
            {
                if (runError.ToString() == "IDLE_10M")
                {
                    log.Warn(runError.ToString());
                    //webSocketWrap.Close(WebSocketColseCodes.ActivityIDLE_10M);
                }
                else
                {
                    log.Warn("【警告】OnActivityAction runEor:" + runError);
                }
            }
        }

        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
