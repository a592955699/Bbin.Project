using Bbin.Core.Entitys;
using Bbin.Core.Cons;
using Bbin.Data;
using System;
using Bbin.Core.Extensions;
using log4net;
using Newtonsoft.Json;
using Bbin.Core.Model;
using System.Collections.Generic;

namespace Bbin.Result
{
    public class ResultService : IResultService
    {

        private readonly IResultDbService resultDbService;
        private readonly IGameDbService gameDbService;
        private readonly IMQService mqService;
        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(ResultService));
        //private Dictionary<string, GameEntity> gameDic = new Dictionary<string, GameEntity>();
        
        //private GameEntity GetGameCache(string roomId)
        //{
        //    return gameDic.GetValueOrDefault(roomId);
        //}

        //private void PutGameCache(GameEntity gameEntity)
        //{
        //    gameDic[gameEntity.RoomId] = gameEntity;
        //}
        //private void RemoveGameCache(string roomId)
        //{
        //    gameDic.Remove(roomId);
        //}

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
            mqService.ListenerManager((queueModel) => {
                try
                {
                    if (queueModel == null || queueModel.Data == null)
                    {
                        log.Warn("【警告】Round 转 Result 失败,数据不完整！");
                        return;
                    }
                    var round = queueModel.Data;
                    if (string.IsNullOrWhiteSpace(round?.Rn))
                    {
                        log.Warn($"【警告】Round 转 Result 失败,数据不完整！{JsonConvert.SerializeObject(round)}");
                        return;
                    }
                    ResultEntity result = round.ToResult();
                    if (result == null)
                    {
                        log.Warn($"【警告】Round 转 Result 失败！{ JsonConvert.SerializeObject(round)}");
                        return;
                    }
                    if (resultDbService.FindByRs(result.Rs) != null)
                    {
                        log.Info($"【提示】结果已存在！跳过后续操作！{JsonConvert.SerializeObject(round)}");
                        return;
                    }

                    GetGame(round, result, out GameEntity game, out bool isNes);
                    if (game == null)
                    {
                        log.Warn($"【警告】处理结果失败！game=null Json:{JsonConvert.SerializeObject(round)}");
                        return;
                    }

                    result.Game = game;

                    //#TODO 事物处理
                    if (isNes)
                    {
                        if (gameDbService.FindByDateAndIndex(game.RoomId, game.Date, game.Index) == null)
                        {
                            gameDbService.Insert(game);
                            log.Info($"【提示】靴不存在！新增靴! {JsonConvert.SerializeObject(result)}");
                        }
                        else
                        {
                            log.Info($"【提示】靴已存在！不新增靴!{JsonConvert.SerializeObject(result)}");
                        }
                    }
                    //if (resultDbService.findByRs(result.Rs) != null)
                    //{
                    //    log.InfoFormat($"【提示】结果已存在！跳过后续操作!Rs: {result.Rs}");
                    //    return;
                    //}
                    //else
                    //{
                    //    resultDbService.Insert(result);
                    //}

                    resultDbService.Insert(result);

                    //log.DebugFormat("【提示】结果处理完毕！Json: {0}", JsonConvert.SerializeObject(round));

                    //处理完毕，推送通知
                    //注意：按照备份模式推送。 推送多个副本。
                    //1.处理好路
                    //2.处理下注
                    mqService.PublishResult(result.Rs);
                    log.Info($"【提示】推送 Result 通知完毕 Rs: {result.Rs}");
                }
                catch (Exception ex)
                {
                    log.Warn("【警告】侦听 round 处理结果异常！", ex);
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
                    RoomId = result.Game.RoomId,
                    DateTime = result.Begin,
                    Date = result.Game.Date,
                    Index = result.Game.Index
                };

                log.InfoFormat("【提示】采集到 Round 结果(新的一靴) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index,round.Pk);
                return;
            }
            else
            {
                var preResult = resultDbService.FindResult(result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index - 1);
                //连续的近两局（且间隔不超过2小时）,表示是同一靴
                //获取上一局的结果中的 Game
                if (preResult != null)
                {
                    if((DateTime.Now - preResult.Game.DateTime).TotalHours>2)
                    {
                        isNew = true;
                        game = new GameEntity()
                        {
                            RoomId = result.Game.RoomId,
                            DateTime = result.Begin,
                            Date = result.Game.Date,
                            Index = result.Game.Index
                        };

                        log.InfoFormat("【提示】采集到 Round 结果(新的一靴,上一靴超过2小时) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
                        return;
                    }

                    game = preResult.Game;
                    log.InfoFormat("【提示】采集到 Round 结果(连续) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
                    return;
                }

                //连续的两局，但是是跨天了
                //获取上一局的结果中的 Game
                if (preResult == null)
                {
                    preResult = resultDbService.FindResult(result.Game.RoomId, result.Begin.AddDays(-1).ToString("yyyyMMdd"), result.Game.Index, result.Index - 1);
                    if (preResult != null && (round.Begin - preResult.Begin).TotalMinutes < 5)//相差5分钟内的，算同一局
                    {
                        game = preResult.Game;
                        log.InfoFormat("【提示】采集到 Round 结果(跨天连续) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
                        return;
                    }
                }

                //这一桌 date 的最后一靴
                var lastGame = gameDbService.FindLastGame(result.Game.RoomId, result.Game.Date);
                if (lastGame != null)
                {
                    //当前靴是 date 的最后一靴
                    //获取最后一靴的 Game
                    if (lastGame.Index == result.Game.Index)
                    {
                        game = lastGame;

                        log.InfoFormat("【提示】采集到 Round 结果(当前靴是 date 的最后一靴) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
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

                        log.InfoFormat("【提示】采集到 Round 结果(中途开始采集) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
                        return;
                    }
                }


                //这一桌 yesterdayDate 的最后一靴
                var yesterdayLastGame = gameDbService.FindLastGame(result.Game.RoomId, result.Game.Date);
                if (yesterdayLastGame != null)
                {
                    //当前靴是 yesterdayDate 的最后一靴
                    //获取最后一靴的 Game
                    if (yesterdayLastGame.Index == result.Game.Index)
                    {
                        game = yesterdayLastGame;
                        log.InfoFormat("【提示】采集到 Round 结果(当前靴是 date 的最后一靴【跨天】) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
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
                log.InfoFormat("【提示】采集到 Round 结果(中途开始采集) RoomId:{0} Game Date:{1} Game Index:{2} Round Index:{3} Pk:{4}",
                        result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
            }
        }
    }
}
