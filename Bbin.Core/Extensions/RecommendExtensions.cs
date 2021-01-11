﻿using Bbin.Core.Entitys;
using Bbin.Core.Enums;
using Bbin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (recommendTemplate == null || recommendTemplate.Items == null || recommendTemplate.Items.Count == 0)
                return false;

            if (resultEntities.Count < recommendTemplate.Items.Sum(x => x.Times))
                return false;

            //倒序处理
            var items = recommendTemplate.Items.OrderBy(x => x.Id).ToList();
            var results = resultEntities.OrderBy(x => x.Rn).ToList();

            var match = true;


            RecommendItemEntity lastItem;
            ResultEntity lastResult;
            //处理过的结果数量，每判断一个结果，就递增+1
            int skipResultIndex = 0;
            for (int itemsIndex = items.Count - 1; itemsIndex >= 0; itemsIndex--)
            {
                #region 循环模板
                #region 循环模板次数
                lastItem = items[itemsIndex];
                for (int timesIndex = 0; timesIndex < lastItem.Times; timesIndex++)
                {
                    #region 循环结果
                    for (int resultsIndex = results.Count - 1 - (skipResultIndex++); resultsIndex >= 0; resultsIndex--)
                    {
                        lastResult = results[resultsIndex];

                        if(lastResult.ResultState==ResultState.UnKnown)
                        {
                            //匹配任意一个结果
                            continue;
                        }
                        if (lastResult.ResultState == ResultState.He)
                        {
                            #region 处理结果为“和”的情况
                            //如果结果是和，则取上一个的庄闲
                            //如果上一个也是和，继续再去取上一个，以此类推
                            //第一个结果是和，没法计算
                            if (resultsIndex == 0)
                                return false;

                            //上一个结果的索引
                            int preResultsIndex = resultsIndex;
                            ResultEntity preResult;

                            var tempMath = false;
                            while (--preResultsIndex>=0)
                            {
                                preResult = results[preResultsIndex];
                                if(preResult.ResultState==ResultState.He)
                                {
                                    continue;
                                }
                                if(preResult.ResultState== lastItem.ResultState)
                                {
                                    tempMath = true;                                    
                                }
                                break;
                            }
                            if (!tempMath)
                                return false;
                            #endregion
                        }
                        else
                        {
                            #region 处理庄闲
                            if (lastResult.ResultState != lastItem.ResultState)
                                return false;
                            else
                            {                               
                                break;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                #endregion
                #endregion
            }

            return match;
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
