using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Sniffer.Cons;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer.SnifferActionExecutors
{
    /// <summary>
    /// 抽象 ws 侦听执行器
    /// 注意：此处为同步类 
    /// </summary>
    public abstract class AbstractWsActionExecutor : IActionExecutor
    {
        protected ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(AbstractWsActionExecutor));
        public Dictionary<string, object> Data { get; private set; }
        public ISocketService SocketService { get;private set; }
        public object DoExecute(params object[] args)
        {
            object runError = string.Empty;
            if (Data.TryGetValue("runEor", out runError))
            {
                log.Warn("【提示】ActionExecutor runEor:" + runError);

                if (runError.ToString() == "IDLE_5M")
                {
                    SocketService.Close(WebSocketColseCodes.ActivityIDLE_5M);
                }
                else if (runError.ToString() == "IDLE_10M")
                {
                    SocketService.Close(WebSocketColseCodes.ActivityIDLE_10M);
                }
                else
                {
                    SocketService.Close(WebSocketColseCodes.API_EC_ACC_SID_INVALID);
                }
                return null;
            }
            Execute(args);
            return null;
        }
        public void SetData(Dictionary<string, object>  data, ISocketService socketService)
        {
            Data = data;
            SocketService = socketService;
        }
        public virtual void SetParams<T>(string name, T param)
        {

        }
        public abstract void Execute(params object[] args);
    }
}
