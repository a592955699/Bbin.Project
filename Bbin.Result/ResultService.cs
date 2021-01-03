﻿using Bbin.Api.Baccarat.Entitys;
using Bbin.Core.Cons;
using Bbin.Core.RabbitMQ;
using Bbin.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Bbin.Api.Baccarat.Extensions;
using log4net;
using Newtonsoft.Json;

namespace Bbin.Result
{
    public class ResultService : IResultService
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        private readonly IResultDbService resultDbService;
        private readonly IGameDbService gameDbService;
        private readonly RabbitMQClient rabbitMQClient;

        ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(ResultService));

        public ResultService(RabbitMQConfig _rabbitMQConfig,
            IResultDbService _resultDbService, 
            IGameDbService _gameDbService,
            RabbitMQClient _rabbitMQClient)
        {
            this.rabbitMQConfig = _rabbitMQConfig;
            this.resultDbService = _resultDbService;
            this.gameDbService = _gameDbService;
            this.rabbitMQClient = _rabbitMQClient;
        }
        public void Listener()
        {
            RabbitMQListener.QueueListener<RoundModel>(rabbitMQConfig, RabbitMQCons.SnifferRoundQueue, true, (round) =>
            {
                try
                {
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

                    if (isNes)
                        gameDbService.Insert(game);
                    resultDbService.Insert(result);

                    //处理完毕，推送通知
                    //注意：按照备份模式推送。 推送多个副本。
                    //1.处理好路
                    //2.处理下注
                    rabbitMQClient.SendQueue(result.ResultId, RabbitMQCons.SnifferResuleQueue);
                }
                catch (Exception ex)
                {
                    log.WarnFormat("【警告】处理结果异常！Json: {0}", JsonConvert.SerializeObject(round), ex);
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
