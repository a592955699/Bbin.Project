using Bbin.Core;
using Bbin.Core.Cons;
using log4net;

namespace Bbin.Sniffer.ActionExecutors
{
    public class StartSnifferActionExecutor : IActionExecutor
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(StartSnifferActionExecutor));
        public object DoExcute(object args)
        {
            var snifferService = (ISnifferService)ApplicationContext.ServiceProvider.GetService(typeof(ISnifferService));

            //修改了配置，需要重启
            if (args != null && snifferService.Work)
            {
                log.Info("【提示】修改启动参数，准备停止采集任务");
                snifferService.Stop();
                log.Info("【提示】已停止采集任务");
            }

            if (snifferService.Work)
            {
                log.Info("【提示】采集任务已启动");
            }
            else
            {
                log.Info("【提示】准备启动采集任务");
                snifferService.Start();
            }
            return null;
        }
    }
}
