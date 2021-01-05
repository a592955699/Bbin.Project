using System;
using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Result;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bbin.ResultWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("��ӭʹ�� BBIN ���ݲɼ����Թ���!������ֻ����ѧϰ����ʹ�ã�����������ҵ��;��");
            ApplicationContext.ConfigureLog4Net(true);
            ApplicationContext.ConfigureAppsettingsJson();
            ApplicationContext.ConfigureEncodingProvider();

            var log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, Log4NetCons.Name);

            log.Info("************ ��������,������ֻ����ѧϰ����ʹ�ã�����������ҵ��; ************");
            Console.WriteLine("************ ��������,������ֻ����ѧϰ����ʹ�ã�����������ҵ��; ************");


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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


    }
}
