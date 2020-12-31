using Bbin.Api.Baccarat.Configs;
using Bbin.BaiduAI.Config;
using Bbin.Core;
using Bbin.Sniffer;
using Microsoft.Extensions.Configuration;
using System;

namespace Bbin.SnifferConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用 BBIN 百家乐数据采集测试工具!本程序只是做学习交流使用，请勿用于商业用途！");
            ApplicationContext.ConfigureLog4Net();
            ApplicationContext.ConfigureAppsettingsJson();
            ApplicationContext.ConfigureEncodingProvider();

            Console.WriteLine("************ 程序开始侦听 WebSocket");

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            // Duplicate here any configuration sources you use.
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();
            ApplicationContext.Configure(configuration);

            var baiduConfig = configuration.GetSection("baidu").Get<BaiduConfig>();
            var bbinConfig = configuration.GetSection("bbin").Get<BbinConfig>();
            var yongliConfig = configuration.GetSection("yongli").Get<SiteConfig>();

            ISocketService socketService = new SocketService(bbinConfig);
            AbstractLoginService loginService = new YongLiLoginService(yongliConfig, baiduConfig);
            SnifferService snifferService = new SnifferService(loginService, socketService);
            snifferService.DoExecute();

            Console.WriteLine("************ 输入 ‘e’ 退出程序");
            ConsoleKeyInfo key = Console.ReadKey();
            while (key.KeyChar != 'e')
            {
                key = Console.ReadKey();
            }
            //退出时，自动关闭 WS 链接
            socketService.WebSocket.Close();
        }
    }
}
