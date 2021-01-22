using Bbin.Core.Cons;
using Bbin.Core.Entitys;
using Bbin.Core.Enums;
using Bbin.Core.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bbin.Core.Extensions
{
    public static class GameExtensions
    {
        public static GameResultModel ToGameResultModel(this GameEntity game,List<ResultEntity> results)
        {
            if (game == null) return null;
            GameResultModel gameResult = new GameResultModel()
            {
                GameId = game.GameId,
                Index = game.Index,
                RoomId = game.RoomId,
                RoomName = RoomCons.GetRoomName(game.RoomId),
                Date = game.DateTime
            };

            if(results!=null && results.Any())
            {
                var maxIndex = results.Max(x => x.Index);
                gameResult.ColumnResults = results.ToColumnResults(maxIndex);
                gameResult.NumberResults = results.ToNumberResult(maxIndex);
            }
            return gameResult;
        }
        public static List<NumberResultModel> ToNumberResult(this List<ResultEntity> resultEntities,int maxIndex)
        {
            List<NumberResultModel> numberResults = new List<NumberResultModel>();
            if (resultEntities == null) return numberResults;
            ResultEntity temp;
            for (int i = 1; i <= maxIndex; i++)
            {
                temp = resultEntities.FirstOrDefault(x => x.Index == i);
                if (temp == null)
                    numberResults.Add(new NumberResultModel() { Index = i, ResultState = Enums.ResultState.UnKnown });
                else
                    numberResults.Add(new NumberResultModel() { Index = temp.Index, Number = temp.Number, ResultState = temp.ResultState });
            }
            return numberResults;
        }
        public static PushGameResultModel ToPushGameResultModel(this GameResultModel gameResultModel, Dictionary<RecommendTemplateEntity, ResultState> recommend)
        {
            return new PushGameResultModel() { 
                GameId = gameResultModel.GameId,                
                Date = gameResultModel.Date,
                Index= gameResultModel.Index,              
                RoomId= gameResultModel.RoomId,
                RoomName= gameResultModel.RoomName,
                ColumnResults = gameResultModel.ColumnResults,
                NumberResults = gameResultModel.NumberResults,
                Recommend = recommend
            };
        }
    }
}
