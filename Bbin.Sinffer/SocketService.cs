using Bbin.Core.Eventargs;
using Bbin.Core.Cons;
using Bbin.Sniffer.Cons;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using Bbin.Core.Configs;
using Bbin.Core.Model;
using Bbin.SnifferActionExecutors;
using System.Threading.Tasks;
using Bbin.Sniffer.SnifferActionExecutors;

namespace Bbin.Sniffer
{
    /// <summary>
    /// Socket 采集侦听 bbin 数据
    /// </summary>
    public class SocketService: ISocketService
    {
        #region 事件/委托   
        object objectLock = new Object();
        public EventHandler OnCdEvent;
        public EventHandler OnClosedEvent;
        public EventHandler OnFullResultEvent;
        public EventHandler OnDealingResultEvent;
        public EventHandler OnStateChangeEvent;
        event EventHandler ISocketService.OnCd
        {
            add
            {
                lock (objectLock)
                {
                    OnCdEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnCdEvent -= value;
                }
            }
        }
        event EventHandler ISocketService.OnFullResult
        {
            add
            {
                lock (objectLock)
                {
                    OnFullResultEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnFullResultEvent -= value;
                }
            }
        }
        event EventHandler ISocketService.OnDealingResult
        {
            add
            {
                lock (objectLock)
                {
                    OnDealingResultEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnDealingResultEvent -= value;
                }
            }
        }
        event EventHandler ISocketService.OnStateChange
        {
            add
            {
                lock (objectLock)
                {
                    OnStateChangeEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnStateChangeEvent -= value;
                }
            }
        }
        event EventHandler ISocketService.OnClosed
        {
            add
            {
                lock (objectLock)
                {
                    OnClosedEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnClosedEvent -= value;
                }
            }
        }
        #endregion

        #region 构造函数
        public SocketService(BbinConfig config)
        {
            Config = config;
            InitActionExecutors();
            WebSocket = new WebSocket(Config.Url);
            WebSocket.OnClose += WebSocket_OnClose;
            WebSocket.OnMessage += WebSocket_OnMessage;
            WebSocket.OnOpen += WebSocket_OnOpen;
            WebSocket.OnError += WebSocket_OnError;
        }
        #endregion

        #region 属性
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(SocketService));
        public BbinConfig Config { get; set; }
        public string SessionId { get; set; }
        public WebSocket WebSocket { get; private set; }
        public Dictionary<string, AbstractWsActionExecutor> ActionExecutors { get; private set; }
        public int ReConnectionTimes { get; set; }
        public bool IsConnect { get; private set; }
        #endregion

        #region 公共方法

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            Close(WebSocketColseCodes.ManualShutdown);
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="code"></param>
        public void Close(ushort code)
        {
            WebSocket.Close(code);
        }
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            WebSocket.Connect();
        }

        public void Connect(string sessionId)
        {
            this.SessionId = sessionId;
            Connect();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="completed"></param>
        public void Send(string data)
        {
            log.DebugFormat("【提示】发送数据:{0}", data);
            try
            {
                Task.Run(()=> {
                    WebSocket.Send(data);
                });
            }
            catch (Exception e)
            {
                log.Error("【错误】发送数据异常", e);
            }
        }

        public void SetParams<T>(string action, string name, T value)
        {
            AbstractWsActionExecutor actionExecutor = null;
            if (ActionExecutors.TryGetValue(action, out actionExecutor))
            {
                actionExecutor.SetParams<T>(name, value);
                return;
            }
            else
            {
                return;
            }
        }
        public string GetActionName(Dictionary<string, object> keyValues)
        {
            if (keyValues == null)
                return string.Empty;

            if (keyValues.ContainsKey("action"))
            {
                return keyValues["action"].ToString();
            }

            //Ping  NetStatusEvent 格式不一样，需要特别处理
            if (keyValues.ContainsKey(ActionNames.Ping))
            {
                return ActionNames.Ping;
            }
            if (keyValues.ContainsKey(ActionNames.NetStatusEvent))
            {
                return ActionNames.NetStatusEvent;
            }
            return string.Empty;
        }
        public void InternalOnCd(RoundModel roomInfo)
        {
            var args = new CdEventArgs()
            {
                RoomId = roomInfo.RoomId,
                Cd = roomInfo.Cd,
                Rn = roomInfo.Rn,
                Rs = roomInfo.Rs
            };
            OnCdEvent?.Invoke(this, args);
        }
        public void InternalOnFullResult(RoundModel roomInfo)
        {
            try
            {
                var args = new FullResultEventArgs()
                {
                    Round = new RoundModel()
                    {
                        RoomId = roomInfo.RoomId,
                        Pk = roomInfo.Pk,
                        Rn = roomInfo.Rn,
                        Rs = roomInfo.Rs,
                        Begin = roomInfo.Begin,
                        End = roomInfo.End
                    }
                };
                OnFullResultEvent?.Invoke(this, args);
            }
            catch (Exception e)
            {
                log.Error("【错误】InternalOnFullResult 异常", e);
            }
        }
        public void InternalOnDealingResult(RoundModel roomInfo)
        {
            try
            {
                var args = new DealingResultEventArgs()
                {
                    RoomId = roomInfo.RoomId,
                    Pk = roomInfo.Pk,
                    Rn = roomInfo.Rn,
                    Rs = roomInfo.Rs
                };
                OnDealingResultEvent?.Invoke(this, args);
            }
            catch (Exception e)
            {
                log.Error("【错误】InternalOnDealingResult 异常", e);
            }
        }
        public void InternalOnStateChange(RoundModel roomInfo)
        {
            try
            {
                var args = new StateChangeEventArgs()
                {
                    RoomId = roomInfo.RoomId,
                    St = roomInfo.St,
                    Rn = roomInfo.Rn,
                    Rs = roomInfo.Rs
                };
                OnStateChangeEvent?.Invoke(this, args);
            }
            catch (Exception e)
            {
                log.Error("【错误】InternalOnStateChange 异常", e);
            }
        }
        public void InternalOnClosed(CloseEventArgs e)
        {
            try
            {
                OnClosedEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                log.Error("【错误】InternalOnClosed 异常", ex);
            }
        }
        #endregion

        #region WebSocket 事件
        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            log.ErrorFormat("【错误】WebSocket 异常:{0}", JsonConvert.SerializeObject(e));
        }

        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            IsConnect = true;
            lock (objectLock)
            {
                ReConnectionTimes = 0;
            }
            log.Info("【提示】WebSocket 连接成功.");
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                //log.DebugFormat("【提示】接收数据:{0}", e.Data);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
                var actionName = GetActionName(dict);

                AbstractWsActionExecutor actionExecutor;
                if (ActionExecutors.TryGetValue(actionName, out actionExecutor))
                {
                    actionExecutor.SetData(dict, this);
                    actionExecutor.DoExecute(null);
                }
                else
                {
                    log.WarnFormat($"【警告】找不到 ActionName = {0} 对应的 IActionExecutor", e.Data);
                }
            }
            catch (Exception ex)
            {
                log.Error("【错误】WebSocket_OnMessage 异常", ex);
            }
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            IsConnect = false;
            log.WarnFormat("【警告】WebSocket 关闭. Code:{0} Reason:{1}", e.Code, e.Reason);
            try
            {
                InternalOnClosed(e);
            }
            catch (Exception ex)
            {
                log.Error("【错误】WebSocket_OnClose 异常", ex);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 加载配置，正常的应该去配置读取
        /// </summary>
        private void InitActionExecutors()
        {
            ActionExecutors = new Dictionary<string, AbstractWsActionExecutor>();
            ActionExecutors.Add(ActionNames.Ping, new PingAction());
            ActionExecutors.Add(ActionNames.Ready, new ReadyAction());
            ActionExecutors.Add(ActionNames.NetStatusEvent, new NetStatusEventAction());
            ActionExecutors.Add(ActionNames.OnHallLogin, new OnHallLoginAction());
            ActionExecutors.Add(ActionNames.OnPoolInfo, new OnPoolInfoAction());
            ActionExecutors.Add(ActionNames.OnLAS, new OnLASAction());
            ActionExecutors.Add(ActionNames.OnUpdateGameInfo, new OnUpdateGameInfoAction());
            ActionExecutors.Add(ActionNames.OnUserInfo, new OnUserInfoAction());
            ActionExecutors.Add(ActionNames.OnVideoLink, new VideoLinkAction());
            ActionExecutors.Add(ActionNames.Preferences, new PreferencesAction());
            ActionExecutors.Add(ActionNames.OnLogin, new OnLoginAction());
            ActionExecutors.Add(ActionNames.OnGameConfig, new OnGameConfigAction());
            ActionExecutors.Add(ActionNames.OnActivity, new OnActivityAction());
        }

        #endregion
    }
}
