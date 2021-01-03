using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Sniffer
{
    public interface ISnifferService
    {
        /// <summary>
        /// 开始采集
        /// 操作动作：
        /// 1.登录
        /// 2.根据登录获得 bbin session，链接 bbin WS 开始采集
        /// </summary>
        void Start();
        /// <summary>
        /// 停止采集
        /// </summary>
        void Stop();
        /// <summary>
        /// 账号是否登录
        /// </summary>
        /// <returns></returns>
        bool IsLogin();
        /// <summary>
        /// bbin WS 是否连接
        /// </summary>
        /// <returns></returns>
        bool IsConnect();
    }
}
