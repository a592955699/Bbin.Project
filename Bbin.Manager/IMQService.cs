using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Manager
{
    public interface IMQService
    {
        /// <summary>
        /// 侦听 ManagerQueue 
        /// </summary>
        void ListenerManager();
    }
}
