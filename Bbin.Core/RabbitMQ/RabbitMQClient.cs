using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.RabbitMQ
{
    public class RabbitMQClient
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        public RabbitMQClient(RabbitMQConfig _rabbitMQConfig)
        {
            this.rabbitMQConfig = _rabbitMQConfig;
        }

        public void SendQueue<T>(T message, string queue, string exchange = "", string routingKey = "", bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
            where T : new()
        {
            var messageString = JsonConvert.SerializeObject(message);
            SendQueue(messageString, queue, exchange, routingKey, durable, exclusive, autoDelete, arguments);
        }

        public void SendQueue(string message, string queue, string exchange = "", string routingKey = "", bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
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
            using (var connection = factory.CreateConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //声明一个队列
                    channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);

                    var sendBytes = Encoding.UTF8.GetBytes(message);

                    //默认 routingKey 和 queue 一致
                    if (string.IsNullOrWhiteSpace(routingKey))
                        routingKey = queue;

                    //发布消息
                    channel.BasicPublish(exchange, routingKey, null, sendBytes);

                    channel.Close();
                }
                connection.Close();
            }
        }
    }
}
