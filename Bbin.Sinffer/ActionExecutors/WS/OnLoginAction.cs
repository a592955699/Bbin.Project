using Bbin.Sniffer.Cons;
using Bbin.Sniffer;
using System;
using System.Collections.Generic;
using Bbin.Core.Cons;
using log4net;
using Bbin.Sniffer.SnifferActionExecutors;

namespace Bbin.SnifferActionExecutors
{
    public class OnLoginAction : AbstractWsActionExecutor
    {
        protected static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(OnLoginAction));
        public override object DoExecute(params object[] paras)
        {
            object runError = string.Empty;
            if (Data.TryGetValue("runEor", out runError))
            {
                log.Warn("【警告】OnLoginAction runEor:" + runError + Environment.NewLine);
                //if ("API_EC_ACC_SID_INVALID".Equals(runError))
                SocketService.Close(WebSocketColseCodes.API_EC_ACC_SID_INVALID);
            }
            return null;
        }
    }
}
