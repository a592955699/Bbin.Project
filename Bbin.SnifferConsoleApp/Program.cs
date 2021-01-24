using System;
using Bbin.Core;
using Bbin.Core.Cons;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Bbin.Sniffer;
using Bbin.BaiduAI.Config;
using Bbin.Core.Configs;
using Bbin.Core.Commandargs;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Bbin.SnifferConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用 BBIN 数据采集测试工具(Bbin.Sniffer 端)!本程序只是做学习交流使用，请勿用于商业用途！");
            ApplicationContext.ConfigureLog4Net(false);
            ApplicationContext.ConfigureEncodingProvider();

            var log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, Log4NetCons.Name);

            log.Info("************ 本程序只是做学习交流使用，请勿用于商业用途 ************");
            Console.WriteLine("************ 本程序只是做学习交流使用，请勿用于商业用途 ************");

            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                ApplicationContext.ServiceProvider = serviceProvider;
                try
                {
                    var mqService = serviceProvider.GetService<IMQService>();
                    //侦听 ManagerExchange 
                    mqService.ListenerManager();

                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                using (var scope = host.Services.CreateScope())
                                {
                                    var services = scope.ServiceProvider;

                                    var mqService = services.GetService<IMQService>();
                                    var snifferService = ApplicationContext.ServiceProvider.GetService<ISnifferService>();
                                    var siteConfig = services.GetService<SiteConfig>();
                                    SnifferUpArgs snifferUpArgs = new SnifferUpArgs();
                                    snifferUpArgs.QueueName = mqService.QueueName;
                                    snifferUpArgs.UserName = siteConfig.UserName;
                                    snifferUpArgs.PassWord = siteConfig.PassWord;
                                    snifferUpArgs.Connected = snifferService.IsConnect();
                                    snifferUpArgs.Work = snifferService.Work;

                                    //上线通知 ManagerQueue
                                    mqService.PublishUp(snifferUpArgs);
                                    log.Debug($"【提示】已发送上线通知，args:{JsonConvert.SerializeObject(snifferUpArgs)}");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("发送上线通知异常", ex);
                            }
                            await Task.Delay(10000);
                        }
                    });

                    //开始采集
                    //var snifferService = services.GetService<ISnifferService>();
                    //snifferService.Start();
                }
                catch (Exception ex)
                {
                    log.Error("An error occurred while seeding the database.", ex);
                }

                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return new HostBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            config.AddJsonFile("appsettings.Development.json", true);
#else
            config.AddJsonFile("appsettings.json", true);
#endif
            if (args != null) config.AddCommandLine(args);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton(hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
            services.AddSingleton(hostContext.Configuration.GetSection("BaiduConfig").Get<BaiduConfig>());
            services.AddSingleton(hostContext.Configuration.GetSection("BbinConfig").Get<BbinConfig>());
            services.AddSingleton(hostContext.Configuration.GetSection("SiteConfig").Get<SiteConfig>());

            services.AddSingleton<AbstractLoginService, YongLiLoginService>();
            services.AddSingleton<IMQService, RabbitMQService>();
            services.AddSingleton<ISocketService, SocketService>();
            services.AddSingleton<ISnifferService, SnifferService>();

            ApplicationContext.Configuration = hostContext.Configuration;
        });
        }
    }
}
