using Bbin.Core.Cons;
using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.RabbitMQ
{
    public class RabbitMQListener
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(RabbitMQListener));
        //private readonly RabbitMQConfig rabbitMQConfig;
        //public RabbitMQListener(RabbitMQConfig _rabbitMQConfig)
        //{
        //    this.rabbitMQConfig = _rabbitMQConfig;
        //}

        public static void QueueListener<T>(RabbitMQConfig rabbitMQConfig, string queue, bool autoAck, Action<T> receivedAction)
            where T : new()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = rabbitMQConfig.UserName,
                Password = rabbitMQConfig.Password,
                HostName = rabbitMQConfig.HostName,
                Port = rabbitMQConfig.Port,
                VirtualHost = rabbitMQConfig.VirtualHost
            };


            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                log.Debug($"【提示】RabbitMQ Queue:{queue} Message:{message}");

                try
                {
                    receivedAction(JsonConvert.DeserializeObject<T>(message));
                }
                catch (Exception ex)
                {
                    log.Error($"【错误】RabbitMQ 消费消息异常", ex);
                }

                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queue, false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }

        public static void QueueListener(RabbitMQConfig rabbitMQConfig, string queue, bool autoAck, Action<string> receivedAction)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = rabbitMQConfig.UserName,
                Password = rabbitMQConfig.Password,
                HostName = rabbitMQConfig.HostName,
                //Port = rabbitMQConfig.Port,
                //VirtualHost = rabbitMQConfig.VirtualHost
            };


            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                log.Debug($"【提示】RabbitMQ Queue:{queue} Message:{message}");

                try
                {
                    receivedAction(message);
                }
                catch (Exception ex)
                {
                    log.Error($"【错误】RabbitMQ 消费消息异常", ex);
                }

                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queue, false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }
    }
}
