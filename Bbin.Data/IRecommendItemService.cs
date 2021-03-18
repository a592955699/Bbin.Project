using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public interface IRecommendItemService
    {
        List<RecommendItemEntity> FindByRecommendTemplateId(int recommendTemplateId);
        List<RecommendItemEntity> FindAll();
    }
}
