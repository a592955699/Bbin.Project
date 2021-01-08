using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Manager;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bbin.ManagerWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("��ӭʹ�� BBIN ���ݲɼ����Թ���(Bbin.Manager ��)!������ֻ����ѧϰ����ʹ�ã�����������ҵ��;��");
            ApplicationContext.ConfigureLog4Net(false);
            ApplicationContext.ConfigureAppsettingsJson();
            ApplicationContext.ConfigureEncodingProvider();

            var log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, Log4NetCons.Name);

            log.Info("************ ������ֻ����ѧϰ����ʹ�ã�����������ҵ��; ************");
            Console.WriteLine("************ ������ֻ����ѧϰ����ʹ�ã�����������ҵ��; ************");


            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var mqService = services.GetService<IMQService>();
                    mqService.ListenerManager();
                }
                catch (Exception ex)
                {
                    log.Error("An error occurred while seeding the database.", ex);
                }
                host.Run();
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
