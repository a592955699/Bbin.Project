using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bbin.Core.Extensions;
using Bbin.Core.Models.UI;
using Microsoft.AspNetCore.SignalR;


namespace Bbin.ManagerWebApp.Hubs
{
    public class GameHub : Hub<IGameHub>
    {
        ///// <summary>
        ///// 推送结果
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <returns></returns>
        //public async Task PushResult(PushGameResultModel gameResult)
        //{
        //    string groupName = GroupExtension.GetGroupName(gameResult.RoomId);
        //    await Clients.Group(groupName).PushResult(gameResult);
        //}
        /// <summary>
        /// 加入组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Caller.JoinGroupAsync(groupName);
        }
        /// <summary>
        /// 加入组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinGroups(List<string> groupNames)
        {
            foreach (var groupName in groupNames)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

            await Clients.Caller.JoinGroupAsync(String.Join(",", groupNames));
        }
        /// <summary>
        /// 退出组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Caller.LeaveGroupAsync(groupName);
        }
        /// <summary>
        /// 退出组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task LeaveGroups(List<string> groupNames)
        {
            foreach (var groupName in groupNames)
            {
                await Clients.Caller.LeaveGroupAsync(groupName);
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, string.Join(",", groupNames));
        }
        /// <summary>
        /// 推送消息给所有人
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Send(string message)
        {
            await Clients.All.SendAsync(message);
        }
        /// <summary>
        /// 推送消息给其他人
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToOthers(string message)
        {
            await Clients.Others.SendToOthersAsync(message);
        }
        /// <summary>
        /// 推送消息给指定链接
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToConnection(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendToConnectionAsync(message);
        }
        /// <summary>
        /// 推送消息给指定组
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendToGroupAsync(message);
        }
        public async Task SendToOthersInGroup(string groupName, string message)
        {
            await Clients.OthersInGroup(groupName).SendToOthersInGroupAsync(message);
        }
    }
}