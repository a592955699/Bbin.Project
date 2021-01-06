using Bbin.Core;
using Bbin.Core.Configs;
using Bbin.Core.Cons;
using Bbin.Core.Model;
using Bbin.Core.Models;
using Bbin.Sniffer.ActionExecutors;
using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer
{
    public class RabbitMQService : IMQService
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        private readonly Dictionary<string, IActionExecutor> ActionExecutors;
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(RabbitMQService));
        public RabbitMQService(RabbitMQConfig _rabbitMQConfig)
        {
            rabbitMQConfig = _rabbitMQConfig;
            ActionExecutors = GetActionExecutors();
        }

        /// <summary>
        /// fanout 侦听消息
        /// </summary>
        public void ListenerManager()
        {
            //实例化一个连接工厂和其配置为使用所需的主机，虚拟主机和证书（证书）
            ConnectionFactory factory = GetConnectionFactory(rabbitMQConfig);
            //创建一个AMQP 0-9-1连接
            IConnection connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            //同样要声明交换机的类型及名称，不然publish和consumer匹配不上
            channel.ExchangeDeclare(exchange: RabbitMQCons.ManagerExchange, type: "fanout");

            //声明一个队列，这个队列的名称随机
            var queueName = channel.QueueDeclare().QueueName;

            //将这个队列绑定（bind）到交换机上面
            channel.QueueBind(queue: queueName,
                              exchange: RabbitMQCons.ManagerExchange,
                              routingKey: "");

            //声明一个consumer
            var consumer = new EventingBasicConsumer(channel);

            //一个委托，只要这个程序不被杀死，这段代码便一直监听rabbitmq，有消息就实时收到
            consumer.Received += (model, ea) =>
            {
                //处理收到的数据并打印
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);

                var queueModel = JsonConvert.DeserializeObject<QueueModel<RoundModel>>(message);
                if(queueModel == null)
                {
                    log.Warn($"【警告】侦听 exchange {RabbitMQCons.ManagerExchange}  Queue:{queueName}  Body:{message} 接收数据序列化为 null");
                    return;
                }

                IActionExecutor actionExecutor;
                if(ActionExecutors.TryGetValue(queueModel.Key,out actionExecutor))
                {
                    actionExecutor.DoExcute(queueModel.Data);
                }
                else
                {
                    log.Warn($"【警告】侦听 exchange {RabbitMQCons.ManagerExchange}  Queue:{queueName} 不识别命令:{queueModel.Key}");
                    return;
                }
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

        }

        /// <summary>
        /// 发送一个消息 到 RabbitMQCons.RoundQueue 队列，p2p模式
        /// </summary>
        /// <param name="roundModel"></param>
        public void PublishRound(RoundModel roundModel)
        {
            ConnectionFactory factory = GetConnectionFactory(rabbitMQConfig);

            //创建连接
            using (var connection = factory.CreateConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //声明一个队列
                    //channel.QueueDeclare(RabbitMQCons.RoundQueue, false, false, false, null);

                    //封装消息实体
                    var ququeModel = new QueueModel<RoundModel>(CommandKeys.PublishRound, roundModel);

                    //将消息实体转成 json 后，处理成 byte[]
                    var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ququeModel));

                    //发布消息
                    channel.BasicPublish("", RabbitMQCons.RoundQueue, null, sendBytes);
                    channel.Close();
                }
                connection.Close();
            }
        }

        private ConnectionFactory GetConnectionFactory(RabbitMQConfig rabbitMQConfig)
        {
            return  new ConnectionFactory
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
            keyValues.Add(CommandKeys.PublishSnifferStart, new StartSnifferActionExecutor());
            keyValues.Add(CommandKeys.PublishSnifferStop, new StopSnifferActionExecutor());
            return keyValues;
        }
    }
}
