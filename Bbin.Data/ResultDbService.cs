using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Bbin.Core.Entitys;

namespace Bbin.Data
{
    public class ResultDbService : IResultDbService
    {
        private BbinDbContext dbContext;
        public ResultDbService(BbinDbContext _bbinDbContext)
        {
            this.dbContext = _bbinDbContext;
        }
        public ResultEntity findById(long resultId)
        {
            return dbContext.Results.FirstOrDefault(x => x.ResultId == resultId);
        }

        public List<ResultEntity> findList(long gameId)
        {
            return dbContext.Results.Where(x => x.Game.GameId == gameId).OrderByDescending(x=>x.Index).ToList();
        }

        public bool Insert(ResultEntity result)
        {
            EntityEntry<ResultEntity> entityEntry = dbContext.Results.Add(result);
            return dbContext.SaveChanges()>0;
        }

        public ResultEntity GetResult(string roomId, string date, int gameIndex, int index)
        {
            return dbContext.Results.FirstOrDefault(x => x.Game.Index == gameIndex && x.Game.RoomId == roomId && x.Game.Date == date && x.Index == index);
        }


    }
}
