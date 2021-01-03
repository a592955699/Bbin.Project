using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.RabbitMQ
{
    [Serializable]
    public class RabbitMQConfig 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; } = AmqpTcpEndpoint.UseDefaultPort;
        public string VirtualHost { get; set; } = "/";
    }
}
