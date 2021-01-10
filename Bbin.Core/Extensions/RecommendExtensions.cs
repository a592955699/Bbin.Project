using Bbin.Core.Entitys;
using Bbin.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Extensions
{
    public static class RecommendExtensions
    {
        /// <summary>
        /// 是否推荐
        /// </summary>
        /// <param name="resultEntities">结果集合</param>
        /// <param name="recommendTemplate">好路推荐模板设置</param>
        /// <returns></returns>
        public static bool IsRecommend(this List<ResultEntity> resultEntities, RecommendTemplateModel recommendTemplate)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 满足任意一个
        /// </summary>
        /// <param name="resultEntities"></param>
        /// <param name="recommendTemplates"></param>
        /// <returns></returns>
        public static bool IsRecommendAny(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates)
        {
            if (recommendTemplates == null || recommendTemplates.Count == 0) return false;
            foreach (var item in recommendTemplates)
            {
                if (resultEntities.IsRecommend(item))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 满足所有
        /// </summary>
        /// <param name="resultEntities"></param>
        /// <param name="recommendTemplates"></param>
        /// <returns></returns>
        public static bool IsRecommendAll(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates)
        {
            if (recommendTemplates == null || recommendTemplates.Count == 0) return false;
            foreach (var item in recommendTemplates)
            {
                if (!resultEntities.IsRecommend(item))
                    return false;
            }
            return true;
        }
    }
}
