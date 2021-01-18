using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public interface IGameDbService
    {
        GameEntity FindById(long gameId);
        GameEntity FindByDateAndIndex(string date, int index);
        GameEntity FindNext(long gameId);
        GameEntity FindPre(long gameId);
        bool Insert(GameEntity game);

        /// <summary>
        /// 获取最后一靴
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        GameEntity FindLastGame(string roomId, string date);
    }
}
