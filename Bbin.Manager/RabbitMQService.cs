using Bbin.Core;
using Bbin.Core.Configs;
using Bbin.Core.Cons;
using Bbin.Core.Models;
using Bbin.Manager.ActionExecutors;
using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Manager
{
    public class RabbitMQService : IMQService
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(RabbitMQService));
        private readonly Dictionary<string, IActionExecutor> ActionExecutors;
        public RabbitMQService(RabbitMQConfig _rabbitMQConfig)
        {
            rabbitMQConfig = _rabbitMQConfig;
            ActionExecutors = GetActionExecutors();
        }

        public void ListenerManager()
        {
            //实例化一个连接工厂和其配置为使用所需的主机，虚拟主机和证书（证书）
            ConnectionFactory factory = GetConnectionFactory(rabbitMQConfig);

            //创建连接
            var connection = factory.CreateConnection();

            //创建通道
            var channel = connection.CreateModel();

            //声明一个名称消息队列
            channel.QueueDeclare(RabbitMQCons.ManagerQueue, false, false, false, null);

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //启动消费者 设置为自动应答消息
            channel.BasicConsume(RabbitMQCons.ManagerQueue, true, consumer);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    //处理收到的数据并打印
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var queueModel = JsonConvert.DeserializeObject<QueueModel<object>>(message);
                    if (queueModel == null)
                    {
                        log.Warn($"【警告】侦听 Queue:{RabbitMQCons.ManagerQueue}  Body:{message} 接收数据序列化为 null");
                        return;
                    }

                    IActionExecutor actionExecutor;
                    if (ActionExecutors.TryGetValue(queueModel.Key, out actionExecutor))
                    {
                        log.Debug($"【提示】侦听 Queue:{RabbitMQCons.ManagerQueue} 准备执行 Action:{actionExecutor.GetType().Name}");
                        actionExecutor.DoExcute(message);
                    }
                    else
                    {
                        log.Warn($"【警告】侦听 Queue:{RabbitMQCons.ManagerQueue} 不识别命令:{queueModel.Key}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"【错误】侦听 Queue:{RabbitMQCons.ManagerQueue} 异常!",ex);
                    return;
                }
            };
        }

        private ConnectionFactory GetConnectionFactory(RabbitMQConfig rabbitMQConfig)
        {
            return new ConnectionFactory
            {
                UserName = rabbitMQConfig.UserName,
                Password = rabbitMQConfig.Password,
                HostName = rabbitMQConfig.HostName,
                Port = rabbitMQConfig.Port,
                VirtualHost = rabbitMQConfig.VirtualHost
            };
        }

        private Dictionary<string, IActionExecutor> GetActionExecutors()
        {
            Dictionary<string, IActionExecutor> keyValues = new Dictionary<string, IActionExecutor>();
            keyValues.Add(CommandKeys.PublishSnifferUp, new PublishSnifferUpActionExecutor());
            keyValues.Add(CommandKeys.PublishResult, new PublishResultActionExcutor());
            return keyValues;
        }
    }
}
