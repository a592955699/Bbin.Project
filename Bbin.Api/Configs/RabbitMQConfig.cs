using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Api.Configs
{
    [Serializable]
    public class RabbitMQConfig 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; } = -1;
        public string VirtualHost { get; set; } = "/";
    }
}
