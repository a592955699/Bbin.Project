using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Core.RabbitMQ;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace Bbin.ResultConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用 BBIN 百家乐数据采集测试工具!本程序只是做学习交流使用，请勿用于商业用途！");
            ApplicationContext.ConfigureLog4Net(true);
            ApplicationContext.ConfigureAppsettingsJson();
            ApplicationContext.ConfigureEncodingProvider();

            Console.WriteLine("************ 程序开始侦听 WebSocket");

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            // Duplicate here any configuration sources you use.
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();
            ApplicationContext.Configure(configuration);

            var rabbitMQConfig = configuration.GetSection("rabbitMQ").Get<RabbitMQConfig>();
            RabbitMQListener.QueueListener(rabbitMQConfig, RabbitMQCons.ResuleQueue, true, (message) =>
            {
                Console.WriteLine($"模拟消费消息:{message}");
            });


            Console.WriteLine("************ 输入 ‘e’ 退出程序");
            ConsoleKeyInfo key = Console.ReadKey();
            while (key.KeyChar != 'e')
            {
                key = Console.ReadKey();
            }
        }
    }
}
