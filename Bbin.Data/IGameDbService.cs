using Bbin.Api.Baccarat.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public interface IGameDbService
    {
        GameEntity findById(long gameId);
        bool Insert(GameEntity game);

        /// <summary>
        /// 获取最后一靴
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        GameEntity GetLastGame(string roomId, string date);
    }
}
