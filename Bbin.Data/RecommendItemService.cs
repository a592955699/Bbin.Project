using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bbin.Data
{
    public class RecommendItemService : IRecommendItemService
    {
        private BbinDbContext dbContext;
        public RecommendItemService(BbinDbContext _bbinDbContext)
        {
            this.dbContext = _bbinDbContext;
        }

        public List<RecommendItemEntity> FindAll()
        {
            return dbContext.RecommendItems.ToList();
        }

        public List<RecommendItemEntity> FindByRecommendTemplateId(int recommendTemplateId)
        {
            return dbContext.RecommendItems.Where(x => x.RecommendTemplateId == recommendTemplateId).OrderBy(x => x.Id).ToList();
        }
    }
}
