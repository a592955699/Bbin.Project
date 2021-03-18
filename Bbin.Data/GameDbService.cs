using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Bbin.Core.Models;
using Webdiyer.AspNetCore;

namespace Bbin.Data
{
    public class GameDbService : IGameDbService
    {
        private BbinDbContext dbContext;
        public GameDbService(BbinDbContext _bbinDbContext)
        {
            this.dbContext = _bbinDbContext;
        }
        public GameEntity FindById(long gameId)
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
        public GameEntity FindLastGame(string roomId, string date)
        {
            return dbContext.Games.Where(x => x.RoomId == roomId && x.Date == date).OrderByDescending(x => x.DateTime).FirstOrDefault();
        }

        public GameEntity FindByDateAndIndex(string roomId, string date, int index)
        {
            return dbContext.Games.OrderByDescending(x=>x.DateTime).FirstOrDefault(x => x.Date == date && x.Index == index);
        }

        public GameEntity FindNext(long gameId,string roomId)
        {
            return dbContext.Games.Where(x=>x.GameId>gameId && x.RoomId == roomId).OrderBy(x=>x.GameId).FirstOrDefault();
        }

        public GameEntity FindPre(long gameId, string roomId)
        {
            return dbContext.Games.Where(x => x.GameId < gameId && x.RoomId == roomId).OrderByDescending(x => x.GameId).FirstOrDefault();
        }

        public PagedList<GameEntity> FindList(DateTime? start = null, DateTime? end = null, int pageIndex = 1, int pageSize = 10)
        {
            var query = dbContext.Games.AsQueryable();
            if(start!=null && start!=DateTime.MinValue)
            {
                query = query.Where(x => start.Value>= x.DateTime);
            }
            if (end != null && end != DateTime.MinValue)
            {
                query = query.Where(x => x.DateTime <= end.Value);
            }
            query = query.OrderByDescending(x => x.GameId);

            return query.ToPagedList(pageIndex,pageSize);
        }
    }
}
