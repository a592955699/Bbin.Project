using Bbin.Core.Entitys;
using Bbin.Core.Enums;
using Bbin.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Extensions
{
    public static class RoundExtensions
    {
        public static ResultEntity ToResult(this RoundModel baccaratRound)
        {
            ResultEntity baccaratResult = new ResultEntity()
            {
                Begin = baccaratRound.Begin,
                End = baccaratRound.End,
                Rn = baccaratRound.Rn,
                Rs = baccaratRound.Rs,
                Game = new GameEntity() { 
                    RoomId = baccaratRound.RoomId,
                    Index = int.Parse(baccaratRound.Rn.Split("-")[0]),
                    Date = baccaratRound.Begin.ToString("yyyyMMdd")
                }
            };
            baccaratResult.Index = int.Parse(baccaratResult.Rn.Split("-")[1]);

            if (!string.IsNullOrWhiteSpace(baccaratRound.Pk))
            {
                var results = baccaratRound.Pk.Split(",");
                baccaratResult.Card1 = results[0];
                baccaratResult.Card2 = results[2];
                baccaratResult.Card3 = results[4];
                baccaratResult.Card4 = results[1];
                baccaratResult.Card5 = results[3];
                baccaratResult.Card6 = results[5];
            }

            int left = 0, right = 0;
            left += new CardModel(baccaratResult.Card1).Result;
            left += new CardModel(baccaratResult.Card2).Result;
            left += new CardModel(baccaratResult.Card3).Result;
            //var leftY = left % 10;
            //left = leftY >= 0 ? leftY : left;
            left = left % 10;

            right += new CardModel(baccaratResult.Card4).Result;
            right += new CardModel(baccaratResult.Card5).Result;
            right += new CardModel(baccaratResult.Card6).Result;
            //var rightY = right % 10;
            //right = rightY >= 0 ? rightY : right;
            right = right % 10;

            baccaratResult.Number = left > right ? left : right;
            baccaratResult.ResultState = left > right ? ResultState.XianJia : (left == right ? ResultState.He : ResultState.ZhuangJia);
            //baccaratResult.IsBig = baccaratResult.Result > 5;
            return baccaratResult;
        }
    }
}
