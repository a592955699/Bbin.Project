﻿using System;
using System.Configuration;
using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Core.RabbitMQ;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Bbin.Sniffer;
using Bbin.BaiduAI.Config;
using Bbin.Api.Baccarat.Entitys;

namespace Bbin.SnifferConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用 BBIN 百家乐数据采集测试工具!本程序只是做学习交流使用，请勿用于商业用途！");
            ApplicationContext.ConfigureLog4Net(true);
            ApplicationContext.ConfigureAppsettingsJson();
            ApplicationContext.ConfigureEncodingProvider();

            var log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, Log4NetCons.Name);

            log.Info("************ 启动程序,本程序只是做学习交流使用，请勿用于商业用途 ************");
            Console.WriteLine("************ 启动程序,本程序只是做学习交流使用，请勿用于商业用途 ************");


            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var snifferService = services.GetService<ISnifferService>();
                    snifferService.Start();
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
                services.AddSingleton(hostContext.Configuration.GetSection("BaiduConfig").Get<BaiduConfig>());
                services.AddSingleton(hostContext.Configuration.GetSection("BbinConfig").Get<BbinConfig>());
                services.AddSingleton(hostContext.Configuration.GetSection("SiteConfig").Get<SiteConfig>());

                services.AddSingleton<RabbitMQClient>();
                services.AddSingleton<AbstractLoginService, YongLiLoginService>();
                services.AddSingleton<ISocketService, SocketService>();
                services.AddSingleton<ISnifferService, SnifferService>();
            });
    }
}
