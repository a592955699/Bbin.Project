//using Bbin.Core;
//using Bbin.Core.Configs;
//using Bbin.Core.Cons;
//using log4net;
//using Microsoft.Extensions.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Bbin.Core.Commandargs;
//using Newtonsoft.Json;

//namespace Bbin.Sniffer
//{
//    public class HeartbeatService 
//    {
//        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(HeartbeatService));
//        public  void ExecuteAsync()
//        {
//            Task.Run(async ()=>{
//                while (true)
//                {
//                    try
//                    {
//                        //需要执行的任务
//                        var mqService = ApplicationContext.ServiceProvider.GetService<IMQService>();
//                        var socketService = ApplicationContext.ServiceProvider.GetService<ISocketService>();
//                        var siteConfig = ApplicationContext.ServiceProvider.GetService<SiteConfig>();
//                        SnifferUpArgs snifferUpArgs = new SnifferUpArgs();
//                        snifferUpArgs.QueueName = mqService.QueueName;
//                        snifferUpArgs.UserName = siteConfig.UserName;
//                        snifferUpArgs.Password = siteConfig.PassWrod;
//                        snifferUpArgs.Connected = socketService.IsConnect;

//                        //上线通知 ManagerQueue
//                        mqService.PublishUp(snifferUpArgs);
//                        log.Info($"【提示】已发送上线通知，args:{JsonConvert.SerializeObject(snifferUpArgs)}");

//                    }
//                    catch (Exception ex)
//                    {
//                        log.Error("【错误】执行心跳任务异常", ex);
//                    }
//                    await Task.Delay(5000);//等待1秒
//                }

//            });
//        }
//    }
//}
