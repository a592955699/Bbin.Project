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
        public ResultEntity FindByRs(string rs)
        {
            return dbContext.Results.Include(x=>x.Game).FirstOrDefault(x => x.Rs == rs);
        }

        public List<ResultEntity> FindList(long gameId)
        {
            return dbContext.Results.Where(x => x.Game.GameId == gameId).OrderBy(x=>x.Index).ToList();
        }

        public bool Insert(ResultEntity result)
        {
            EntityEntry<ResultEntity> entityEntry = dbContext.Results.Add(result);
            return dbContext.SaveChanges()>0;
        }

        public ResultEntity FindResult(string roomId, string date, int gameIndex, int index)
        {
            return dbContext.Results.OrderByDescending(x=>x.Begin).FirstOrDefault(x => x.Game.Index == gameIndex && x.Game.RoomId == roomId && x.Game.Date == date && x.Index == index);
        }


    }
}
