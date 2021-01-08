using Bbin.Core.Commandargs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bbin.Manager
{
    public class ManagerApplicationContext
    {
        public List<SnifferUpArgs> Sniffers = new List<SnifferUpArgs>();

        /// <summary>
        /// sniffer 上线，添加到集合
        /// </summary>
        /// <param name="snifferUpArgs"></param>
        public void AddSniffers(SnifferUpArgs snifferUpArgs)
        {
            RemoveSniffers(snifferUpArgs);
            Sniffers.Add(snifferUpArgs);
        }

        /// <summary>
        /// sniffer 下线，或者是心跳丢失，移除集合
        /// </summary>
        /// <param name="snifferUpArgs"></param>
        public void RemoveSniffers(SnifferUpArgs snifferUpArgs)
        {
            var sniffer = Sniffers.FirstOrDefault(x => x.QueueName == snifferUpArgs.QueueName);
            if (sniffer != null)
            {
                Sniffers.Remove(sniffer);
            }
        }
    }
}
