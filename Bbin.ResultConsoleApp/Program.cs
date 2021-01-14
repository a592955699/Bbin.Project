using System;
using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Data;
using Bbin.Result;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Bbin.Core.Configs;

namespace Bbin.ResultConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用 BBIN 数据采集测试工具(Bbin.Result 端)!本程序只是做学习交流使用，请勿用于商业用途！");
            ApplicationContext.ConfigureLog4Net(false);
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
                services.AddSingleton(hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
                services.AddScoped<IResultDbService, ResultDbService>();
                services.AddScoped<IGameDbService, GameDbService>();
                services.AddScoped<IMQService, RabbitMQService>();
                services.AddScoped<IResultService, ResultService>();

                //services.AddDbContext<BbinDbContext>(options =>
                //   options.UseSqlServer(hostContext.Configuration.GetConnectionString("BbinDbContext")
                //   , b => b.MigrationsAssembly("Bbin.ResultConsoleApp"))
                //);
             
                services.AddDbContext<BbinDbContext>(options => 
                options.UseMySQL(hostContext.Configuration.GetConnectionString("BbinDbContext")
                   , b => b.MigrationsAssembly("Bbin.ResultConsoleApp")));

                ApplicationContext.Configuration = hostContext.Configuration;
                ApplicationContext.ServiceProvider = services.BuildServiceProvider();
            });
    }
}
