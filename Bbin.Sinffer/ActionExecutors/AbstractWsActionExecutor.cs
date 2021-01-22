using Bbin.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer.SnifferActionExecutors
{
    /// <summary>
    /// 抽象 ws 侦听执行器
    /// 注意：此处为同步类 
    /// </summary>
    public abstract class AbstractWsActionExecutor : IActionExecutor
    {
        public Dictionary<string, object> Data { get; private set; }
        public ISocketService SocketService { get;private set; }
        public abstract object DoExecute(params object[] args);
        public void SetData(Dictionary<string, object>  data, ISocketService socketService)
        {
            Data = data;
            SocketService = socketService;
        }
        public virtual void SetParams<T>(string name, T param)
        {

        }
    }
}
