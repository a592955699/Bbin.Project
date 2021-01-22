using Bbin.Core.Entitys;
using Bbin.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Webdiyer.AspNetCore;

namespace Bbin.Data
{
    public interface IGameDbService
    {
        GameEntity FindById(long gameId);
        GameEntity FindByDateAndIndex(string roomId, string date, int index);
        GameEntity FindNext(long gameId, string roomId);
        GameEntity FindPre(long gameId, string roomId);
        bool Insert(GameEntity game);

        /// <summary>
        /// 获取最后一靴
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        GameEntity FindLastGame(string roomId, string date);

        PagedList<GameEntity> FindList(int pageIndex = 1, int pageSize = 10);
    }
}
