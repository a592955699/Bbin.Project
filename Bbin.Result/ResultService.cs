﻿using Bbin.Core.Entitys;
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
                        log.Warn($"【警告】数据转换失败,原因：数据不完整!");
                        return;
                    }
                    var round = queueModel.Data;
                    if (string.IsNullOrWhiteSpace(round?.Rn))
                    {
                        log.Warn($"【警告】数据转换失败,原因：数据不完整!{JsonConvert.SerializeObject(round)}");
                        return;
                    }
                    ResultEntity result = round.ToResult();
                    if (result == null)
                    {
                        log.Warn($"【警告】数据转换失败,原因：round.ToResult() = null!{ JsonConvert.SerializeObject(round)}");
                        return;
                    }
                    if (resultDbService.FindByRs(result.Rs) != null)
                    {
                        log.Info($"【提示】跳过后续操作,原因：结果已存在！{JsonConvert.SerializeObject(round)}");
                        return;
                    }

                    GetGame(round, result, out GameEntity game, out bool isNes);
                    if (game == null)
                    {
                        log.Warn($"【警告】数据处理失败,原因：game=null Json:{JsonConvert.SerializeObject(round)}");
                        return;
                    }

                    result.Game = game;

                    //#TODO 事物处理
                    if (isNes)
                    {
                        var dbGame = gameDbService.FindByDateAndIndex(game.RoomId, game.Date, game.Index);
                        //判断时间是为了处理同一天存在两个相同 Game Index 问题
                        if (dbGame == null || (DateTime.Now - dbGame.DateTime).TotalHours > 2)
                        {
                            gameDbService.Insert(game);
                            log.Info($"【提示】数据转换成功，新增靴，原因：靴不存在!{JsonConvert.SerializeObject(result)}");
                        }
                        else
                        {
                            log.Info($"【提示】数据转换成功，不新增靴，原因：靴已存在!{JsonConvert.SerializeObject(result)}");
                        }
                    }

                    resultDbService.Insert(result);


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
            /** 处理逻辑：
             *  取 RoomId GameIndex ResultIndex 对应的上一个结果（今天的取不到，则去昨天的）
             *  判断上一个结果和这个结果没有超过10分钟，说明是同一靴
             *  否则都是新一靴
             */

            game = null;
            isNew = true;

            if (result.Index == 1)
            {
                isNew = true;
            }
            else
            {
                var preResult = resultDbService.FindLastResult(result.Game.RoomId, result.Game.Date, result.Game.Index);
                if (preResult==null)
                {
                    //推测跨天，获取前一天的上一个结果
                    preResult = resultDbService.FindLastResult(result.Game.RoomId, result.Begin.AddDays(-1).ToString("yyyyMMdd"), result.Game.Index);
                    if (log.IsDebugEnabled)
                        log.Debug($"【提示】查找到上一天上一个结果,{JsonConvert.SerializeObject(preResult)}");
                }
                else
                {
                    if (log.IsDebugEnabled)
                        log.Debug($"【提示】查找到同一天上一个结果,{JsonConvert.SerializeObject(preResult)}");
                }
                if (preResult != null && (result.Begin - preResult.Begin).TotalMinutes <= 10)
                {
                    isNew = false;
                    game = preResult.Game;

                    log.InfoFormat("【提示】采集到 Round 结果(同一靴) RoomId:{0} Game Index:{2} Round Index:{3} Game Date:{1} Pk:{4}",
                    result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
                }
            }
            if (isNew)
            {
                game = new GameEntity()
                {
                    RoomId = result.Game.RoomId,
                    DateTime = result.Begin,
                    Date = result.Game.Date,
                    Index = result.Game.Index
                };

                log.InfoFormat("【提示】采集到 Round 结果(新的一靴) RoomId:{0} Game Index:{2} Round Index:{3} Game Date:{1} Pk:{4}",
                result.Game.RoomId, result.Game.Date, result.Game.Index, result.Index, round.Pk);
                return;
            }
        }
    }
}
