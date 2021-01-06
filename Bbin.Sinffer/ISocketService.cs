using Bbin.Api.Entitys;
using Bbin.Api.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;

namespace Bbin.Sniffer
{
    public interface ISocketService
    {
        #region 属性
        string SessionId { get; set; }
        WebSocket WebSocket { get; }
        /// <summary>
        /// 处理 ws 响应的执行器
        /// </summary>
        Dictionary<string, IInternalActionExecutor> ActionExecutors { get; }
        object SetParams<T>(string action, string name, T value);
        /// <summary>
        /// 重连次数
        /// </summary>
        int ReConnectionTimes { get; set; }
        /// <summary>
        /// socket 是否连接
        /// </summary>
        bool IsConnect { get; }
        #endregion

        #region 事件
        /// <summary>
        /// 倒计时数秒
        /// </summary>
        event EventHandler OnCd;
        /// <summary>
        /// 出整体结果
        /// </summary>
        event EventHandler OnFullResult;
        /// <summary>
        /// 出部分结果
        /// </summary>
        event EventHandler OnDealingResult;
        /// <summary>
        /// 状态变化
        /// </summary>
        event EventHandler OnStateChange;
        /// <summary>
        /// 链接关闭
        /// </summary>
        event EventHandler OnClosed;
        void InternalOnClosed(CloseEventArgs e);
        void InternalOnCd(RoundModel roomInfo);
        void InternalOnFullResult(RoundModel roomInfo);
        void InternalOnDealingResult(RoundModel roomInfo);
        void InternalOnStateChange(RoundModel roomInfo);
        #endregion

        #region 方法
        void Connect();
        void Connect(string sessionId);
        void Close();
        void Close(ushort code);
        void Send(string data);
        string GetActionName(Dictionary<string, object> keyValues);
        #endregion
    }
}
