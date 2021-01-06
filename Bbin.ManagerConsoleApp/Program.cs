
using Bbin.Core;
using Bbin.Core.Configs;
using Bbin.Core.Cons;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Bbin.ManagerConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用 BBIN 数据采集测试工具(Bbin.Manager 端)!本程序只是做学习交流使用，请勿用于商业用途！");
            ApplicationContext.ConfigureLog4Net(true);
            ApplicationContext.ConfigureAppsettingsJson();
            ApplicationContext.ConfigureEncodingProvider();

            var log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, Log4NetCons.Name);

            log.Info("************ 本程序只是做学习交流使用，请勿用于商业用途 ************");
            Console.WriteLine("************ 本程序只是做学习交流使用，请勿用于商业用途 ************");


            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                   
                }
                catch (Exception ex)
                {
                    log.Error("An error occurred while seeding the database.", ex);
                }
                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
            });
    }
}
