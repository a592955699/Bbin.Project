using Bbin.Core.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bbin.ManagerWebApp.Hubs
{
    public interface IGameHub
    {
        Task PushResult(GameResultModel gameResult);
        Task JoinGroupAsync(string message);
        Task LeaveGroupAsync(string message);
        Task SendAsync(string message);
        Task SendToOthersAsync(string message);
        Task SendToConnectionAsync(string message);
        Task SendToGroupAsync(string message);
        Task SendToOthersInGroupAsync(string message);
    }
}
