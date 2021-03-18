using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bbin.Data
{
    public class RecommendTemplateService : IRecommendTemplateService
    {
        private BbinDbContext dbContext;
        public RecommendTemplateService(BbinDbContext _bbinDbContext)
        {
            this.dbContext = _bbinDbContext;
        }

        public List<RecommendTemplateEntity> FindAll(bool? publish)
        {
            var query = dbContext.RecommendTemplates.AsQueryable();
            if (publish.HasValue)
                query = query.Where(x => x.Publish == publish.Value).OrderByDescending(x => x.Sort);
            return query.ToList();
        }
    }
}
