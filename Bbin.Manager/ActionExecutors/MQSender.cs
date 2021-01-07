using Bbin.Core;
using Bbin.Core.Configs;
using Bbin.Core.Cons;
using Bbin.Core.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Manager.ActionExecutors
{
    public abstract class MQSender 
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        public MQSender(RabbitMQConfig _rabbitMQConfig)
        {
            rabbitMQConfig = _rabbitMQConfig;
        }

        public void SendMessage<T>(string queueName, QueueModel<T> queue)
        {
            ConnectionFactory factory = GetConnectionFactory(rabbitMQConfig);

            //创建连接
            using (var connection = factory.CreateConnection())
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //声明一个队列
                    //channel.QueueDeclare(queueName, false, false, false, null);

                    //将消息实体转成 json 后，处理成 byte[]
                    var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queue));

                    //发布消息
                    channel.BasicPublish("", queueName, null, sendBytes);
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
