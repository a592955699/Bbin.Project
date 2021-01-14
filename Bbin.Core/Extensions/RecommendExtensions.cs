using Bbin.Core.Entitys;
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
        /// 好路推荐
        /// </summary>
        /// <param name="resultEntities">结果集合</param>
        /// <param name="recommendTemplate">好路推荐模板设置</param>
        /// <param name="result">当前结果，用于计算没有采集到结果的时候，匹配 UnKnow</param>
        /// <returns></returns>
        public static bool IsRecommend(this List<ResultEntity> resultEntities, RecommendTemplateModel recommendTemplate,ResultEntity result)
        {
            if (recommendTemplate == null || recommendTemplate.Items == null || recommendTemplate.Items.Count == 0)
                return false;

            if (resultEntities.Count < recommendTemplate.Items.Sum(x => x.Times))
                return false;

            //升序处理
            var items = recommendTemplate.Items.OrderBy(x => x.Id).ToList();
            var results = resultEntities.OrderBy(x=>x.Game.GameId).ThenBy(x => x.Index).ToList();

            #region 按照结果顺序，不同颜色单独放一列
            //最后一列
            List<ResultState> lastCol;
            //最后一个结果状态
            ResultState lastColState;
            ResultState tempState;
            var colResults = new List<List<ResultState>>();


            /**
             * 处理逻辑，进入循环前，新增一列
             * 每次循环，如果列中有结果，则判断结果和之前结果是否匹配，如果不匹配 ，则新增一列
             * 将循环结果加入最后一列中
             */
            colResults.Add(new List<ResultState>());
            for (int i = 1; i <= result.Index; i++)
            {
                //获取最后一列
                lastCol = colResults[colResults.Count - 1];
                lastColState = lastCol[lastCol.Count - 1];

                //数据库没有，默认为 ResultState.UnKnown
                var tempResult = results.FirstOrDefault(x => x.Index == i && x.Game.GameId == result.Game.GameId);
                tempState = tempResult == null ? ResultState.UnKnown : tempResult.ResultState;
                if (lastCol.Any())//最后一列有结果，且最后一个结果 State ！= tempState,新增列
                {
                    if(tempState == ResultState.He)
                    {
                        tempState = lastColState;
                    }
                    else if (lastColState != tempState)
                    {
                        colResults.Add(new List<ResultState>());
                        lastCol = colResults[colResults.Count - 1];
                    }
                }
                lastCol.Add(tempState);
            }

            //最后一列没数据，则清空
            if (colResults.Any() && !colResults[colResults.Count - 1].Any())
            {
                colResults.RemoveAt(colResults.Count - 1);
            }
            #endregion

            //没结果列，则返回 false
            if (!colResults.Any())
            {
                return false;
            }

            RecommendItemEntity lastRecommendItem;
            //特殊处理长龙
            if (recommendTemplate.Items.Count == 1)
            {
                lastRecommendItem = recommendTemplate.Items.FirstOrDefault();
                lastCol = colResults[colResults.Count - 1];
                lastColState = lastCol[lastCol.Count - 1];
                return lastCol.Count >= lastRecommendItem.Times && lastColState == lastRecommendItem.ResultState;
            }

            int colSkip = 0;
            var match = true;
            //循环处理结果列集合，倒序匹配推荐模板
            for (int itemIndex = recommendTemplate.Items.Count - 1; itemIndex >= 0; itemIndex--)
            {
                lastCol = colResults[colResults.Count - 1 - (colSkip++)];
                lastColState = lastCol[lastCol.Count - 1];

                lastRecommendItem = recommendTemplate.Items[itemIndex];
                if (lastRecommendItem.ResultState != lastColState && lastRecommendItem.Times != lastCol.Count)
                {
                    match = false;
                    break;
                }
            }

            #region 
            /*
                RecommendItemEntity lastItem;
                ResultEntity lastResult = null;
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
                            //两次结果是否有间隙
                            if (lastResult != null && lastResult.Index - results[resultsIndex].Index != 1)
                            {
                                return false;
                            }
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
                */
            #endregion
            return match;
        }
        /// <summary>
        /// 满足任意一个好路推荐
        /// </summary>
        /// <param name="resultEntities"></param>
        /// <param name="recommendTemplates"></param>
        /// <returns></returns>
        public static bool IsRecommendAny(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates, ResultEntity result)
        {
            if (recommendTemplates == null || recommendTemplates.Count == 0) return false;
            foreach (var item in recommendTemplates)
            {
                if (resultEntities.IsRecommend(item, result))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 满足所有好路推荐
        /// </summary>
        /// <param name="resultEntities"></param>
        /// <param name="recommendTemplates"></param>
        /// <returns></returns>
        public static bool IsRecommendAll(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates, ResultEntity result)
        {
            if (recommendTemplates == null || recommendTemplates.Count == 0) return false;
            foreach (var item in recommendTemplates)
            {
                if (!resultEntities.IsRecommend(item, result))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 推荐下注设置
        /// </summary>
        /// <param name="recommendTemplate"></param>
        /// <param name="betState"></param>
        /// <returns></returns>
        public static bool IsRecommendBet(this RecommendTemplateModel recommendTemplate,out ResultState betState)
        {
            betState = ResultState.UnKnown;
            if (recommendTemplate.Items == null || recommendTemplate.Items.Count == 0)
                return false;
            var first = recommendTemplate.Items.OrderBy(x => x.Id).FirstOrDefault();
            if (first.ResultState == ResultState.UnKnown || first.ResultState == ResultState.He)
                return false;
            if (recommendTemplate.Template.RecommendType == RecommendTypeEnum.Break)
            {
                betState = (first.ResultState == ResultState.XianJia) ? ResultState.ZhuangJia : ResultState.XianJia;
            }
            else
            {
                betState = first.ResultState;
            }
            return true;
        }

        public static List<RecommendTemplateModel> ToRecommendTemplateModel(List<RecommendTemplateEntity> recommendTemplates,List<RecommendItemEntity> recommendItems)
        {
            
            List<RecommendTemplateModel> recommendTemplateModels = new List<RecommendTemplateModel>();
            if (recommendTemplates == null || recommendItems == null)
                return recommendTemplateModels;
            foreach (var template in recommendTemplates)
            {
                var items = recommendItems.Where(x => x.RecommendTemplateId == template.Id).OrderBy(x => x.Id).ToList();
                if(items.Any())
                {
                    recommendTemplateModels.Add(new RecommendTemplateModel(template,items));
                }
            }
            return recommendTemplateModels;
        }
    }
}
