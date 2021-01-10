using Bbin.Core.Entitys;
using System.Collections.Generic;

namespace Bbin.Core.Models
{
    public class RecommendTemplateModel
    {
        public RecommendTemplateModel() { }

        public RecommendTemplateModel(RecommendTemplateEntity recommendTemplateEntity, List<RecommendItemEntity>  itemEntities) 
        {
            Template = recommendTemplateEntity;
            Items = itemEntities;
        }
        /// <summary>
        /// 推荐模板
        /// </summary>
        public RecommendTemplateEntity Template { get; set; }
        /// <summary>
        /// 推荐模板规则项
        /// </summary>
        public List<RecommendItemEntity> Items { get; set; }
    }
}
