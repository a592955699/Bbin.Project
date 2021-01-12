using Bbin.Core;
using Bbin.Core.Commandargs;
using Bbin.Core.Configs;
using Bbin.Core.Cons;
using Bbin.Core.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Bbin.Manager.ActionExecutors
{
    public class PublishSnifferUpActionExecutor : IActionExecutor
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(PublishSnifferUpActionExecutor));
        public object DoExcute(object args)
        {
            string jsonString = args.ToString();
            var queueModel = JsonConvert.DeserializeObject<QueueModel<SnifferUpArgs>>(jsonString);
            
            var managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();
            managerApplicationContext.AddSniffers(queueModel.Data);

            //#TODO 模拟发动采集申请
            //var newQueueModel = new QueueModel<SnifferUpArgs>(CommandKeys.PublishSnifferStart, queueModel.Data);
            //RabbitMQUtils.SendMessage(queueModel.Data.QueueName, newQueueModel);
            return null;
        }
    }
}
