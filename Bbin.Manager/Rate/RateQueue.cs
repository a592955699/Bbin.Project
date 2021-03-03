using Bbin.Core;
using Bbin.Core.Entitys;
using Bbin.Core.Models;
using Bbin.Data;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Webdiyer.AspNetCore;
using Bbin.Core.Extensions;
using log4net;
using Bbin.Core.Cons;
using Newtonsoft.Json;

namespace Bbin.Manager.Rate
{
    public class RateQueue
    {
        private IGameDbService _gameDbService;
        private IResultDbService _resultDbService;

        public RateQueue(
            IGameDbService gameDbService,
            IResultDbService resultDbService)
        {
            _gameDbService = gameDbService;
            _resultDbService = resultDbService;
            Task.Run(()=> {                
                RateRequest request;
                while (true)
                {
                    try
                    {
                        if (blockingCollection.TryTake(out request, 5))
                        {
                            var rate = StatisticalRateByDate(request.Start, request.End, request.RecommendTemplateModels);
                            rate.RateRequest = request;

                            //#TODO 持久化处理
                            log.Info("计算胜率");
                            log.Info(JsonConvert.SerializeObject(rate));
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            });
        }


        public readonly BlockingCollection<RateRequest> blockingCollection = new BlockingCollection<RateRequest>();
        private static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(RateQueue));

        private Rate StatisticalRateByDate(DateTime start, DateTime end, List<RecommendTemplateModel> recommendTemplateModels)
        {
            int pageIndex = 1;
            int pageSize = 10;
            PagedList<GameEntity> pageList = null;
            int win = 0;
            int lose = 0;
            int he = 0;
            int total = 0;
            _gameDbService = ApplicationContext.ServiceProvider.GetService<IGameDbService>();
            _resultDbService = ApplicationContext.ServiceProvider.GetService<IResultDbService>();
            do
            {
                pageList = _gameDbService.FindList(start: start, end: end, pageIndex: pageIndex, pageSize: pageSize);
                foreach (var game in pageList.ToList())
                {
                    var results = _resultDbService.FindList(game.GameId);

                    var recommendBets = results.StatisticalProbability(recommendTemplateModels);
                    total += recommendBets.Count();
                    win += recommendBets.Where(x => x.RecommendState == x.ResultState).Count();
                    he += recommendBets.Where(x => x.RecommendState != x.ResultState && x.ResultState == Core.Enums.ResultState.He).Count();
                    lose += recommendBets.Where(x => x.RecommendState != x.ResultState && x.ResultState != Core.Enums.ResultState.He).Count();

                    log.Debug($"StatisticalRateByDate {pageIndex}/{pageList.TotalPageCount} total:{total} win:{win} lose:{lose} he:{he}");
                }
            }
            while ((++pageIndex) <= pageList.TotalPageCount);
            double rate = 0;
            if (total != 0)
                rate = win / (double)total * 100;
            return new Rate() { Win = win, Lose = lose, Total = total, He = he, WinRate = rate };
        }

        public bool TryAdd(RateRequest request)
        {
            return blockingCollection.TryAdd(request);
        }

        public bool TryTake(out RateRequest request)
        {
            return blockingCollection.TryTake(out request);
        }
    }
}
