using Bbin.Core.Cons;
using Bbin.Core.Entitys;
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
                GameId=game.GameId,
                Index=game.Index,
                RoomId=game.RoomId,
                RoomName = RoomCons.GetRoomName(game.RoomId)
            };

            if(results!=null && results.Any())
            {
                var maxIndex = results.Max(x => x.Index);
                gameResult.ColumnResults = results.ToColumnResults(maxIndex);
                gameResult.NumberResults = results.Select(x => new NumberResultModel() { Index = x.Index, Number = x.Number, ResultState = x.ResultState }).ToList();
            }
            return gameResult;
        }
    }
}
