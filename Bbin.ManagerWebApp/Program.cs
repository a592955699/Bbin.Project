using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Core.Entitys;
using Bbin.Core.Extensions;
using Bbin.Core.Models;
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
            ////�����Ƽ����
            //List<ResultEntity> resultEntities = new List<ResultEntity>() {
            //    new ResultEntity(){ Index=1,ResultState=Core.Enums.ResultState.He},
            //    new ResultEntity(){ Index=2,ResultState=Core.Enums.ResultState.He},
            //    new ResultEntity(){ Index=3,ResultState=Core.Enums.ResultState.ZhuangJia},
            //    new ResultEntity(){ Index=4,ResultState=Core.Enums.ResultState.ZhuangJia},
            //    new ResultEntity(){ Index=4,ResultState=Core.Enums.ResultState.ZhuangJia},
            //};
            //RecommendTemplateModel templateModel = new RecommendTemplateModel()
            //{
            //    Template = new RecommendTemplateEntity() { Id = 1, Name = "����" },
            //    Items = new List<RecommendItemEntity>() {
            //        new RecommendItemEntity(){ Id=1,ResultState=Core.Enums.ResultState.ZhuangJia,Times=5},
            //    }
            //};
            //var test =  resultEntities.IsRecommend(templateModel);


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
                })
            .UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = false;
            })
            ;
    }
}
