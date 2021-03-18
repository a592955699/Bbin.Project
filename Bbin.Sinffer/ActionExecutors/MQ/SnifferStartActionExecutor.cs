using Bbin.Core;
using Bbin.Core.Commandargs;
using Bbin.Core.Configs;
using Bbin.Core.Cons;
using log4net;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Bbin.Sniffer.ActionExecutors
{
    public class SnifferStartActionExecutor : IActionExecutor
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(SnifferStartActionExecutor));
        public object DoExecute(params object[] args)
        {
            var snifferUpArgs = ((JObject)args[0]).ToObject<SnifferUpArgs>();            
            var siteConfig = ApplicationContext.ServiceProvider.GetService<SiteConfig>();
            siteConfig.UserName = snifferUpArgs.UserName;
            siteConfig.PassWord = snifferUpArgs.PassWord;

            var snifferService = (ISnifferService)ApplicationContext.ServiceProvider.GetService(typeof(ISnifferService));
            snifferService.SetSiteConfig(siteConfig);

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
