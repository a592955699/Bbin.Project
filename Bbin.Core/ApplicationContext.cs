using Bbin.Core.Cons;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace Bbin.Core
{
    public static class ApplicationContext
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Log4net 日志记录
        /// </summary>
        public static ILog Log { get; private set; }

        #region 方法

        /// <summary>
        /// 初始化Log4Net
        /// </summary>
        /// <returns></returns>
        public static void ConfigureLog4Net(bool basicConfigurator = false)
        {
            //日志注册
            var LoggerRepository = LogManager.CreateRepository(Log4NetCons.LoggerRepositoryName);
            if (basicConfigurator)
            {
                // 默认简单配置，输出至控制台
                BasicConfigurator.Configure(LoggerRepository);
            }
            else
            {
                FileInfo file = new FileInfo("log4net.config");
                XmlConfigurator.Configure(LoggerRepository, file);
                Console.WriteLine("************ Log4Net 配置文件:" + file.FullName);
            }

            Log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, Log4NetCons.Name);
        }

        /// <summary>
        /// 编码注册
        /// </summary>
        public static void ConfigureEncodingProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        #endregion
    }
}
