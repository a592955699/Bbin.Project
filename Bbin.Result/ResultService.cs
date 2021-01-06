using Bbin.Api.Entitys;
using Bbin.Api.Cons;
using Bbin.Data;
using System;
using Bbin.Api.Extensions;
using log4net;
using Newtonsoft.Json;
using Bbin.Api.Model;

namespace Bbin.Result
{
    public class ResultService : IResultService
    {
    
        private readonly IResultDbService resultDbService;
        private readonly IGameDbService gameDbService;
        private readonly IMQService mqService;
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(ResultService));

        public ResultService(
            IResultDbService _resultDbService,
            IGameDbService _gameDbService,
            IMQService _mqService
           )
        {
            this.resultDbService = _resultDbService;
            this.gameDbService = _gameDbService;
            this.mqService = _mqService;
        }
        public void Listener()
        {
            mqService.ListenerManager((queueModel)=> {
                try
                {
                    if(queueModel==null || queueModel.Data == null)
                    {
                        log.WarnFormat("【警告】Round 转 Result 失败,数据不完整！");
                        return;
                    }
                    var round = queueModel.Data;
                    if (string.IsNullOrWhiteSpace(round?.Rn))
                    {
                        log.WarnFormat("【警告】Round 转 Result 失败,数据不完整！ {0}", JsonConvert.SerializeObject(round));
                        return;
                    }
                    ResultEntity result = round.ToResult();
                    if (result == null)
                    {
                        log.WarnFormat("【警告】Round 转 Result 失败！ {0}", JsonConvert.SerializeObject(round));
                        return;
                    }

                    GetGame(round, result, out GameEntity game, out bool isNes);
                    if (game == null)
                    {
                        log.WarnFormat("【警告】处理结果失败！game=null Json: {0}", JsonConvert.SerializeObject(round));
                        return;
                    }

                    result.Game = game;

                    //#TODO 事物处理
                    if (isNes)
                        gameDbService.Insert(game);
                    resultDbService.Insert(result);
                    //log.DebugFormat("【提示】结果处理完毕！Json: {0}", JsonConvert.SerializeObject(round));

                    //处理完毕，推送通知
                    //注意：按照备份模式推送。 推送多个副本。
                    //1.处理好路
                    //2.处理下注
                    mqService.PublishResult(result.ResultId);
                    log.DebugFormat($"【提示】推送 Result 通知完毕 ResultId: {result.ResultId}");
                }
                catch (Exception ex)
                {
                    log.WarnFormat("【警告】侦听 round 处理结果异常！", ex);
                    return;
                }
            });
        }

        void GetGame(RoundModel round, ResultEntity result, out GameEntity game, out bool isNew)
        {
            game = null;
            isNew = false;

            //从新开始的一靴
            if (result.Index == 1)
            {
                isNew = true;
                game = new GameEntity()
                {
                    Index = result.Index,
                    DateTime = result.Begin,
                    Date = result.Game.Date,
                    RoomId = result.Game.RoomId
                };
            }
            else
            {
                var preResult = resultDbService.GetResult(result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index - 1);
                //连续的近两局,表示是同一靴
                //获取上一局的结果中的 Game
                if (preResult != null)
                {
                    game = preResult.Game;
                    log.InfoFormat("【提示】采集到 Round 结果(连续) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index);
                    return;
                }

                //连续的两局，但是是跨天了
                //获取上一局的结果中的 Game
                if (preResult == null)
                {
                    preResult = resultDbService.GetResult(result.Game.RoomId, result.Begin.AddDays(-1).ToString("yyyyMMdd"), result.Game.Index, result.Index - 1);
                    if (preResult != null && (round.Begin - preResult.Begin).TotalSeconds < 50)
                    {
                        game = preResult.Game;
                        log.InfoFormat("【提示】采集到 Round 结果(跨天连续) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index);
                        return;
                    }
                }

                //这一桌 date 的最后一靴
                var lastGame = gameDbService.GetLastGame(result.Game.RoomId, result.Game.Date);
                if (lastGame != null)
                {
                    //当前靴是 date 的最后一靴
                    //获取最后一靴的 Game
                    if (lastGame.Index == result.Game.Index)
                    {
                        game = lastGame;

                        log.InfoFormat("【提示】采集到 Round 结果(当前靴是 date 的最后一靴) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index);
                        return;
                    }
                    else
                    {
                        isNew = true;
                        game = new GameEntity()
                        {
                            RoomId = result.Game.RoomId,
                            DateTime = result.Begin,
                            Date = result.Game.Date,
                            Index = result.Game.Index
                        };

                        log.InfoFormat("【提示】采集到 Round 结果(中途开始采集) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index);
                        return;
                    }
                }


                //这一桌 yesterdayDate 的最后一靴
                var yesterdayLastGame = gameDbService.GetLastGame(result.Game.RoomId, result.Game.Date);
                if (yesterdayLastGame != null)
                {
                    //当前靴是 yesterdayDate 的最后一靴
                    //获取最后一靴的 Game
                    if (yesterdayLastGame.Index == result.Game.Index)
                    {
                        game = yesterdayLastGame;
                        log.InfoFormat("【提示】采集到 Round 结果(当前靴是 date 的最后一靴【跨天】) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index);
                        return;
                    }
                }

                game = new GameEntity()
                {
                    RoomId = round.RoomId,
                    DateTime = result.Begin,
                    Date = result.Game.Date,
                    Index = result.Game.Index
                };
                isNew = true;
                log.InfoFormat("【提示】采集到 Round 结果(中途开始采集) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index);
            }
        }
    }
}
