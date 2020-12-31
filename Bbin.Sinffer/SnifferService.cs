using Bbin.Api.Baccarat.Eventargs;
using Bbin.Core.Cons;
using Bbin.Core.RabbitMQ;
using Bbin.Sniffer.Cons;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace Bbin.Sniffer
{
    /// <summary>
    /// 结果采集服务入口
    /// </summary>
    public class SnifferService
    {
        private readonly AbstractLoginService loginService;
        private readonly ISocketService socketService;
        private readonly RabbitMQClient rabbitMQClient;
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(SnifferService));

        /// <summary>
        /// 是否进入循环登录模式
        /// </summary>
        public bool work = true;

        public SnifferService(AbstractLoginService _loginService, ISocketService _socketService, RabbitMQClient _rabbitMQClient)
        {
            this.loginService = _loginService;
            this.socketService = _socketService;
            this.rabbitMQClient = _rabbitMQClient;


            this.socketService.OnCd += WebSocketWrap_OnCd;
            this.socketService.OnDealingResult += WebSocketWrap_OnDealingResult;
            this.socketService.OnFullResult += WebSocketWrap_OnFullResult;
            this.socketService.OnStateChange += WebSocketWrap_OnStateChange;
            this.socketService.OnClosed += WebSocketWrap_OnClosed;
        }

        /// <summary>
        /// 开始启动采集流程
        /// </summary>
        public void DoExecute()
        {
            do
            {
                bool loginState = loginService.CheckAndLogin();
                if (!loginState)
                {
                    log.Warn("【警告】账号认证失败，无法链接ws.等待10S后重试");
                    Thread.Sleep(10000);
                }

                loginService.InternalGetSessionId();
                socketService.Connect(loginService.SessionId);
                break;
            }
            while (work);
        }

        public bool IsLogin()
        {
            return loginService.CheckLogin();
        }

        /// <summary>
        /// 是否连接 bbin ws
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {
            return socketService.IsConnect;
        }

        #region WebSocketWrap 事件
        private void WebSocketWrap_OnStateChange(object sender, EventArgs e)
        {
            var eventArgs = (StateChangeEventArgs)e;
            log.Debug($"【提示】状态改变 {eventArgs.RoomId} Rn:{eventArgs.Rn} Rs:{eventArgs.Rs}  St:{eventArgs.St}");
        }

        private void WebSocketWrap_OnFullResult(object sender, EventArgs e)
        {
            var eventArgs = (FullResultEventArgs)e;            
            log.Info($"【提示】采集全部结果 {eventArgs.Round.RoomId} Rn:{eventArgs.Round.Rn} Rs:{eventArgs.Round.Rs} Pk:{eventArgs.Round.Pk}");
            rabbitMQClient.SendQueue(JsonConvert.SerializeObject(eventArgs.Round), RabbitMQCons.ResuleQueue);
        }

        private void WebSocketWrap_OnDealingResult(object sender, EventArgs e)
        {
            var eventArgs = (DealingResultEventArgs)e;
            log.Debug($"【提示】发牌 {eventArgs.RoomId} Rn:{eventArgs.Rn} Rs:{eventArgs.Rs} 部分结果 {eventArgs.Pk}");
        }

        private void WebSocketWrap_OnCd(object sender, EventArgs e)
        {
            var eventArgs = (CdEventArgs)e;
            log.Debug($"【提示】倒计时 {eventArgs.RoomId} Rn:{eventArgs.Rn} Rs:{eventArgs.Rs} 倒计时 {eventArgs.Cd}");
        }

        private void WebSocketWrap_OnClosed(object sender, EventArgs e)
        {
            var eventArgs = (CloseEventArgs)e;

            //账号 SessionId 过期
            if (eventArgs.Code == WebSocketColseCodes.API_EC_ACC_SID_INVALID)
            {
                log.Warn($"【警告】账号 SessionId 过期,开始自动重连！");
                DoExecute();
                return;
            }
            //手动关闭
            if (eventArgs.Code == WebSocketColseCodes.ManualShutdown)
            {
                log.Warn($"【警告】手动断开 ws 链接！");
                return;
            }
            //网络不稳定
            if (eventArgs.Code == (ushort)CloseStatusCode.Abnormal)
            {
                if ((++socketService.ReConnectionTimes) % 5 == 0)
                {
                    log.Warn($"【警告】网络不稳定，重新次数过多，等待30秒！");
                    Thread.Sleep(30 * 1000);
                }
                log.Warn($"【警告】网络不稳定，开始重新连接！");
                //自动重连
                socketService.Connect();
                return;
            }
            log.Warn($"【警告】已断开 ws 链接！Code:{eventArgs.Code} Reason:{eventArgs.Reason}，开始重新连接！");
            //其他原因，自动重连
            socketService.Connect();
        }
        #endregion
    }
}
