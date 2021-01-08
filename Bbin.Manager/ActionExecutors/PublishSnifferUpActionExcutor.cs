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

namespace Bbin.Manager.ActionExecutors
{
    public class PublishSnifferUpActionExecutor : MQSender,IActionExecutor
    {
        public PublishSnifferUpActionExecutor(RabbitMQConfig _rabbitMQConfig) : base(_rabbitMQConfig)
        {
        }


        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(PublishSnifferUpActionExecutor));
        public object DoExcute(object args)
        {
            string jsonString = args.ToString();
            var queueModel = JsonConvert.DeserializeObject<QueueModel<SnifferUpArgs>>(jsonString);

            var newQueueModel = new QueueModel<SnifferUpArgs>(CommandKeys.PublishSnifferStart, queueModel.Data);

            var queueName = RabbitMQCons.SnifferQueuePrefix + queueModel.Data.Id;

            SendMessage(queueName, newQueueModel);
            return null;
        }
    }
}
