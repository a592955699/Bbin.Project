using System;
using System.Configuration;
using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Core.RabbitMQ;
using Bbin.Data;
using Bbin.Result;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Bbin.ResultConsoleApp
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

                    IResultService resultService = services.GetService<IResultService>();
                    resultService.Listener();
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
                ApplicationContext.Configure(hostContext.Configuration);
                ApplicationContext.Configure(services.BuildServiceProvider());

                services.AddSingleton(hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
                services.AddSingleton<RabbitMQClient>();
                services.AddScoped<IResultDbService, ResultDbService>();
                services.AddScoped<IGameDbService, GameDbService>();
                services.AddScoped<IResultService, ResultService>();

                services.AddDbContext<BbinDbContext>(options =>
                   options.UseSqlServer(hostContext.Configuration.GetConnectionString("BbinDbContext")
                   , b => b.MigrationsAssembly("Bbin.ResultConsoleApp"))
                );

                //services.AddDbContext<BbinDbContext>(options =>
                //   options.UseMySql(Configuration.GetConnectionString("BbinDbContext")
                //   , b => b.MigrationsAssembly("Bbin.ResultWebApp")));
            });
    }
}
