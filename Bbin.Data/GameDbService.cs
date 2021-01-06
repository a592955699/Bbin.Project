using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace Bbin.Data
{
    public class GameDbService : IGameDbService
    {
        private BbinDbContext dbContext;
        public GameDbService(BbinDbContext _bbinDbContext)
        {
            this.dbContext = _bbinDbContext;
        }
        public GameEntity findById(long gameId)
        {
            return dbContext.Games.FirstOrDefault(x => x.GameId == gameId);
        }
        public bool Insert(GameEntity game)
        {
            EntityEntry<GameEntity> entityEntry =  dbContext.Games.Add(game);
            return dbContext.SaveChanges()>0;
        }

        /// <summary>
        /// 获取最后一靴
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public GameEntity GetLastGame(string roomId, string date)
        {
            return dbContext.Games.Where(x => x.RoomId == roomId && x.Date == date).OrderByDescending(x => x.DateTime).FirstOrDefault();
        }
    }
}
