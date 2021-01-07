using Bbin.Sniffer.Cons;
using Bbin.Sniffer;
using System;
using System.Collections.Generic;
using Bbin.Core.Cons;
using log4net;

namespace Bbin.SnifferInternalActionExecutors
{
    public class OnLoginAction : IInternalActionExecutor
    {
        protected static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(OnLoginAction));
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
            object runError = string.Empty;
            if (data.TryGetValue("runEor", out runError))
            {
                log.Warn("【警告】OnLoginAction runEor:" + runError + Environment.NewLine);
                //if ("API_EC_ACC_SID_INVALID".Equals(runError))
                webSocketWrap.Close(WebSocketColseCodes.API_EC_ACC_SID_INVALID);
            }
        }

        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
