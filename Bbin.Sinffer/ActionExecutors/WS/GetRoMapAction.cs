using Bbin.Core.Cons;
using Bbin.Sniffer.Cons;
using Bbin.Sniffer.SnifferActionExecutors;
using log4net;

namespace Bbin.SnifferActionExecutors
{
    public class GetRoMapAction : AbstractWsActionExecutor
    {
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(GetRoMapAction));
        public override object DoExecute(params object[] paras)
        {
            object runError = string.Empty;
            if (Data.TryGetValue("runEor", out runError))
            {
                if (runError.ToString() == "IDLE_10M")
                {
                    log.Info("【提示】"+WebSocketColseCodes.ActivityIDLE_10M);
                }
            }
            return null;
        }
    }
}
