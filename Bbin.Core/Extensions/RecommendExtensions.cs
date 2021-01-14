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
        public static List<List<ResultState>> ToColumnStates(this List<ResultEntity> resultEntities, int resultIndex)
        {
            if (resultEntities == null || !resultEntities.Any())
                return new List<List<ResultState>>();

            var results = resultEntities.OrderBy(x => x.Index).ToList();
            var result = resultEntities[resultEntities.Count - 1];
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
            for (int i = 1; i <= resultIndex; i++)
            {
                //获取最后一列
                lastCol = colResults[colResults.Count - 1];   

                //数据库没有，默认为 ResultState.UnKnown
                var tempResult = results.FirstOrDefault(x => x.Index == i );
                tempState = tempResult == null ? ResultState.UnKnown : tempResult.ResultState;
                if (lastCol.Any())//最后一列有结果，且最后一个结果 State ！= tempState,新增列
                {
                    lastColState = lastCol[lastCol.Count - 1];
                    if (tempState == ResultState.He)
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
            return colResults;
        }

        /// <summary>
        /// 好路推荐
        /// </summary>
        /// <param name="resultEntities">结果集合</param>
        /// <param name="recommendTemplate">好路推荐模板设置</param>
        /// <param name="result">当前结果，用于计算没有采集到结果的时候，匹配 UnKnow</param>
        /// <returns></returns>
        public static bool IsRecommend(this List<ResultEntity> resultEntities, RecommendTemplateModel recommendTemplate,int resultIndex)
        {
            if (recommendTemplate == null || recommendTemplate.Items == null || recommendTemplate.Items.Count == 0)
                return false;

            if (resultEntities.Count < recommendTemplate.Items.Sum(x => x.Times))
                return false;

            //升序处理
            var items = recommendTemplate.Items.OrderBy(x => x.Id).ToList();

            var colResults = resultEntities.ToColumnStates(resultIndex);
            //没结果列，则返回 false
            if (!colResults.Any())
            {
                return false;
            }

            //最后一列
            List<ResultState> lastCol= colResults[colResults.Count-1];
            //最后一个结果状态
            ResultState lastColState= lastCol[lastCol.Count-1];

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

                //去除和了后，如果没数据，则跳过
                var tempCol = lastCol.Where(x => x != ResultState.He).ToList();
                if(!tempCol.Any())
                {
                    continue;
                }

                lastColState = tempCol[tempCol.Count - 1];

                lastRecommendItem = recommendTemplate.Items[itemIndex];
                if (lastRecommendItem.ResultState != lastColState || lastRecommendItem.Times != tempCol.Count)
                {
                    match = false;
                    break;
                }
            }

            return match;
        }
        /// <summary>
        /// 满足任意一个好路推荐
        /// </summary>
        /// <param name="resultEntities"></param>
        /// <param name="recommendTemplates"></param>
        /// <returns></returns>
        public static bool IsRecommendAny(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates, int resultIndex)
        {
            if (recommendTemplates == null || recommendTemplates.Count == 0) return false;
            foreach (var item in recommendTemplates)
            {
                if (resultEntities.IsRecommend(item, resultIndex))
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
        public static bool IsRecommendAll(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates, int resultIndex)
        {
            if (recommendTemplates == null || recommendTemplates.Count == 0) return false;
            foreach (var item in recommendTemplates)
            {
                if (!resultEntities.IsRecommend(item, resultIndex))
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
