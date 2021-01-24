using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Core.Models;
using Bbin.Data;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Bbin.Core.Extensions;
using Bbin.Core.Enums;
using Bbin.Core.Entitys;
using Microsoft.AspNetCore.SignalR;
using Bbin.ManagerWebApp.Hubs;
using static Bbin.Core.Models.UI.PushGameResultModel;

namespace Bbin.Manager.ActionExecutors
{
    public class PublishResultActionExcutor : IActionExecutor
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(PublishResultActionExcutor));

        //private IHubContext<GameHub> HubContext { get; set; }
        //public PublishResultActionExcutor(IHubContext<GameHub> hubContext)
        //{
        //    HubContext = hubContext;
        //}
        public object DoExecute(params object[] args)
        {
            string jsonString = args[0].ToString();
            var queueModel = JsonConvert.DeserializeObject<QueueModel<string>>(jsonString);
                
            var rs =  queueModel.Data;
            var resultDbService = ApplicationContext.ServiceProvider.GetService<IResultDbService>();
            var gameDbService = ApplicationContext.ServiceProvider.GetService<IGameDbService>();
            var result = resultDbService.FindByRs(rs);
            if (result == null)
            {
                log.Warn($"【警告】rs {rs} 找不到对应的结果！");
                return null;
            }

            var results = resultDbService.FindList(result.Game.GameId);
            if (results == null || results.Count == 0)
            {
                log.Warn($"【警告】rs:{rs} 对应的 GameId:{result.Game.GameId} 找不到对应的结果！");
                return null;
            }

            var last =  results.OrderByDescending(x => x.Index).FirstOrDefault();
            if(last.Index> result.Index)
            {
                log.Warn($"【警告】rs:{rs} 对应的 GameId:{result.Game.GameId} 中，最新的结果为 rs:{last.Rs}！此结果是历史结果，不予处理");
                return null;
            }
            var managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();

            List<RecommendResultItem> recommendResultItems = new List<RecommendResultItem>();
            ResultState betState;
            foreach (var recommendTemplateModel in managerApplicationContext.RecommendTemplateModels)
            {
                if(results.IsRecommend(recommendTemplateModel, result.Index) && recommendTemplateModel.IsRecommendBet(out betState))
                {
                    recommendResultItems.Add(new RecommendResultItem() { Id= recommendTemplateModel.Template.Id,Name= recommendTemplateModel .Template.Name,ResultState=betState, RecommendType  = recommendTemplateModel.Template.RecommendType});
                    log.Info($"【提示】Room :{result.Game.RoomId} ({RoomCons.GetRoomName(result.Game.RoomId)}) Name:{recommendTemplateModel.Template.Name} 推荐策略 {recommendTemplateModel.Template.RecommendType} Id:{recommendTemplateModel.Template.Id}  推荐下注 { betState}！ GameId:{result.Game.GameId} rs:{rs}");
                    Console.WriteLine($"【提示】Room :{result.Game.RoomId} ({RoomCons.GetRoomName(result.Game.RoomId)}) Name:{recommendTemplateModel.Template.Name} 推荐策略 {recommendTemplateModel.Template.RecommendType} Id:{recommendTemplateModel.Template.Id}  推荐下注 { betState}！ GameId:{result.Game.GameId} rs:{rs}");
                }
            }

            //#TODO 推送推荐下注

            var hubContext = ApplicationContext.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();

            var resultModel = result.Game.ToGameResultModel(results);
            var pushResultModel = resultModel.ToPushGameResultModel(recommendResultItems);
            string groupName = GroupExtension.GetGroupName(result.Game.RoomId);
            //HubContext.Clients.Groups(groupName).SendAsync(HubCons.PushResult, pushResultModel);
            log.Debug("推送数据到 ui");
            hubContext.Clients.All.SendAsync(HubCons.PushResult, pushResultModel);
            return null;
        }
    }
}
