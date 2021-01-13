﻿using Bbin.Core;
using Bbin.Core.Cons;
using Bbin.Core.Models;
using Bbin.Data;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Bbin.Core.Extensions;
using Bbin.Core.Enums;

namespace Bbin.Manager.ActionExecutors
{
    public class PublishResultActionExcutor : IActionExecutor
    {
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(PublishResultActionExcutor));
        public object DoExcute(object args)
        {
            string jsonString = args.ToString();
            var queueModel = JsonConvert.DeserializeObject<QueueModel<string>>(jsonString);

            var rs =  queueModel.Data;
            var resultDbService = ApplicationContext.ServiceProvider.GetService<IResultDbService>();
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

            Dictionary<int, ResultState> recommendBet = new Dictionary<int, ResultState>();
            ResultState betState;
            foreach (var recommendTemplateModel in managerApplicationContext.RecommendTemplateModels)
            {
                if(results.IsRecommend(recommendTemplateModel) && recommendTemplateModel.IsRecommendBet(out betState))
                {
                    recommendBet.Add(recommendTemplateModel.Template.Id, betState);
                    log.Info($"【提示】rs:{rs} 对应的 GameId:{result.Game.GameId} 推荐下注 {betState}！推荐策略 Id:{recommendTemplateModel.Template.Id} Name:{recommendTemplateModel.Template.Name}");
                    Console.WriteLine($"【提示】rs:{rs} 对应的 GameId:{result.Game.GameId} 推荐下注 {betState}！");
                }
            }

            //#TODO 推送推荐下注
            return null;
        }
    }
}
