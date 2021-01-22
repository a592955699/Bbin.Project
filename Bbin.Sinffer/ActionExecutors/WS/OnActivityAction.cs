
using Bbin.Core.Cons;
using Bbin.Sniffer;
using Bbin.Sniffer.SnifferActionExecutors;
using log4net;
using System.Collections.Generic;

namespace Bbin.SnifferActionExecutors
{
    public class OnActivityAction : AbstractWsActionExecutor
    {
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(OnActivityAction));
        public override object DoExecute( params object[] paras)
        {
            object runError = string.Empty;
            if (Data.TryGetValue("runEor", out runError))
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
            return null;
        }
    }
}
