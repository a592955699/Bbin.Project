using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public interface IRecommendTemplateService
    {
        List<RecommendTemplateEntity> FindAll(bool? publish);
    }
}
