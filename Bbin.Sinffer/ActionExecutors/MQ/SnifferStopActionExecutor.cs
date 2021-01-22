using Bbin.Core;
using Bbin.Core.Cons;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer.ActionExecutors
{
    public class SnifferStopActionExecutor : IActionExecutor
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(SnifferStartActionExecutor));
        public object DoExecute(params object[] args)
        {
            var snifferService = (ISnifferService)ApplicationContext.ServiceProvider.GetService(typeof(ISnifferService));

            if (snifferService.Work)
            {
                log.Info("【提示】准备停止采集任务");
                snifferService.Stop();
            }
            else
            {
                log.Info("【提示】采集任务已停止，跳过本次命令");
            }
            return null;
        }
    }
}
