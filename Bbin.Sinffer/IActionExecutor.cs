using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer
{
    public interface IActionExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">ws接受到的数据转成字典后的结果</param>
        /// <param name="webSocketWrap"></param>
        /// <param name="paras"></param>
        void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras);
        bool SetParams<T>(string name, T param, params object[] paras);
    }
}
