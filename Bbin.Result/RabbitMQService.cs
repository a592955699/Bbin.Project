using Bbin.Core.Configs;
using Bbin.Core.Cons;
using Bbin.Core.Model;
using Bbin.Core.Models;
using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Bbin.Result
{
    public class RabbitMQService : IMQService
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(RabbitMQService));
        public RabbitMQService(RabbitMQConfig _rabbitMQConfig)
        {
            rabbitMQConfig = _rabbitMQConfig;
        }

        public void ListenerManager(Action<QueueModel<RoundModel>> action)
        {
            //实例化一个连接工厂和其配置为使用所需的主机，虚拟主机和证书（证书）
            ConnectionFactory factory = GetConnectionFactory(rabbitMQConfig);

            //创建连接
            var connection = factory.CreateConnection();

            //创建通道
            var channel = connection.CreateModel();

            //声明一个名称消息队列
            channel.QueueDeclare(RabbitMQCons.RoundQueue, false, false, false, null);

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //启动消费者 设置为自动应答消息
            channel.BasicConsume(RabbitMQCons.RoundQueue, true, consumer);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    var queueModel = JsonConvert.DeserializeObject<QueueModel<RoundModel>>(message);

                    action(queueModel);
                }
                catch (Exception ex)
                {
                    log.WarnFormat("【警告】处理结果异常！接收内容: {0}", message, ex);
                    return;
                }
            };
        }

        public void PublishResult(string rs)
        {
            //实例化一个连接工厂和其配置为使用所需的主机，虚拟主机和证书（证书）
            ConnectionFactory factory = GetConnectionFactory(rabbitMQConfig);

            //创建连接
            using (var connection = factory.CreateConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //声明一个队列
                    channel.QueueDeclare(RabbitMQCons.ManagerQueue, false, false, false, null);

                    //封装数据对象
                    var queueModel = new QueueModel<string>(CommandKeys.PublishResult, rs);
                    var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queueModel));


                    //发布消息
                    channel.BasicPublish("", RabbitMQCons.ManagerQueue, null, sendBytes);

                    channel.Close();
                }
                connection.Close();
            }
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
    }
}
