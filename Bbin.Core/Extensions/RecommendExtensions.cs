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
        public static List<List<ResultState>> ToColumnResults(this List<ResultEntity> resultEntities, int resultIndex)
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
                var tempResult = results.FirstOrDefault(x => x.Index == i);
                tempState = tempResult == null ? ResultState.UnKnown : tempResult.ResultState;
                if (lastCol.Any())//最后一列有结果，且最后一个结果 State ！= tempState,新增列
                {
                    lastColState = lastCol[lastCol.Count - 1];
                    if (tempState == ResultState.He)//当前号码是和，则直接往列里面添加
                    {

                    }
                    else if (lastColState == ResultState.He)//上个号码是和，则取最后一个不是和 lastColState
                    {
                        lastColState = lastCol.LastOrDefault(x => x != ResultState.He);
                        if (lastColState != tempState)//lastColState 和 tempState 不一致，则开新列
                        {
                            colResults.Add(new List<ResultState>());
                            lastCol = colResults[colResults.Count - 1];
                        }
                    }
                    else if (lastColState != tempState)//lastColState 和 tempState 不一致，则开新列
                    {
                        colResults.Add(new List<ResultState>());
                        lastCol = colResults[colResults.Count - 1];
                    }
                }
                lastCol.Add(tempState);
                if (lastCol.Count % 6 == 0)
                {
                    colResults.Add(new List<ResultState>());
                }
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
        public static bool IsRecommend(this List<ResultEntity> resultEntities, RecommendTemplateModel recommendTemplate, int resultIndex)
        {
            if (recommendTemplate == null || recommendTemplate.Items == null || recommendTemplate.Items.Count == 0)
                return false;

            if (resultEntities.Count < recommendTemplate.Items.Sum(x => x.Times))
                return false;

            //升序处理
            var items = recommendTemplate.Items.OrderBy(x => x.Id).ToList();

            var colResults = resultEntities.ToColumnResults(resultIndex);
            //没结果列，则返回 false
            if (!colResults.Any())
            {
                return false;
            }

            //最后一列
            List<ResultState> lastCol = colResults[colResults.Count - 1];
            //最后一个结果状态
            ResultState lastColState = lastCol[lastCol.Count - 1];

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
                if (!tempCol.Any())
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
        /// 根据推荐设置计算推荐结果
        /// 注意：需要调用 IsRecommend 判断是否推荐
        /// </summary>
        /// <param name="recommendTemplate"></param>
        /// <param name="betState"></param>
        /// <returns></returns>
        public static bool IsRecommendBet(this RecommendTemplateModel recommendTemplate, out ResultState betState)
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
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="recommendTemplates"></param>
        /// <param name="recommendItems"></param>
        /// <returns></returns>
        public static List<RecommendTemplateModel> ToRecommendTemplateModel(List<RecommendTemplateEntity> recommendTemplates, List<RecommendItemEntity> recommendItems)
        {

            List<RecommendTemplateModel> recommendTemplateModels = new List<RecommendTemplateModel>();
            if (recommendTemplates == null || recommendItems == null)
                return recommendTemplateModels;
            foreach (var template in recommendTemplates)
            {
                var items = recommendItems.Where(x => x.RecommendTemplateId == template.Id).OrderBy(x => x.Id).ToList();
                if (items.Any())
                {
                    recommendTemplateModels.Add(new RecommendTemplateModel(template, items));
                }
            }
            return recommendTemplateModels;
        }


        public static List<RecommendBetModel> StatisticalProbability(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates, int reTryTimes = 2)
        {
            //通过风险控制设置计算的推荐结果
            List<RecommendBetModel> riskRecommendBets = new List<RecommendBetModel>();
            //不通过风险控制设置计算的推荐结果
            List<RecommendItem> noRiskRecommendBets = new List<RecommendItem>();

            var maxIndex = resultEntities.Max(x => x.Index);
            ResultState recommendState;
            ResultState resultState;
            string recommendName="";
            bool isNoRiskBet;
            bool isRiskBet;
            RecommendItem preNoRiskRecommend;
            //按顺序循环处理结果
            for (int index = 1; index <= maxIndex; index++)
            {
                isNoRiskBet = false;
                isRiskBet = false;
                recommendState = ResultState.UnKnown;
                resultState = ResultState.UnKnown;
                recommendName = "";
                #region 按顺序处理推荐规则
                //临时结果列表
                var tempResults = resultEntities.Where(x => x.Index < index).ToList();
                #region 计算是否有匹配到好路推荐设置的数据
                foreach (var recommendTemplate in recommendTemplates)
                {
                    if (tempResults.IsRecommend(recommendTemplate, index - 1) && recommendTemplate.IsRecommendBet(out recommendState))
                    {
                        noRiskRecommendBets.Add(new RecommendItem()
                        {
                            Index = index,
                            RecommendState = recommendState,
                            RecommendTemplateName = recommendTemplate.Template.Name
                        });
                        recommendName = recommendTemplate.Template.Name;
                        isNoRiskBet = true;
                    }
                } 
                #endregion
                //没有符合条件的推荐规则，直接退出循环，进入下一个号码
                if (!isNoRiskBet)
                {
                    continue;
                }
                #region 有符合好路推荐的数据，根据风险设置，计算是否下注
                //不够重复次数的好路推荐推荐，直接推荐下注
                if(index<reTryTimes)
                {
                    isRiskBet = true;
                }
                else
                {
                    /* 推荐下注描述
                       1.推荐规则和上一局推荐规则是否一致
                         一致：
                               判断连续一致的结果中，是否有下注，且下注次数超过 reTryTimes
                                   没有下注或者下注次数没有超过：则直接推荐下注
                                   下注次数超过：不推荐下注
                         不一致：
                               则直接推荐下注
                    */
                    //当前好路推荐规则和上一个的好路推荐规则不一致，直接推荐下注
                    preNoRiskRecommend = noRiskRecommendBets.FirstOrDefault(x => x.Index == index - 1);
                    if (preNoRiskRecommend == null || (preNoRiskRecommend.RecommendState == recommendState && preNoRiskRecommend.RecommendTemplateName == recommendName))
                    {
                        isRiskBet = true;
                    }
                    else
                    {
                        //存储和最近一局相同规则的连续的好路推荐结果
                        var tempNearNoRiskRecommends = new List<RecommendItem>();
                        tempNearNoRiskRecommends.Add(preNoRiskRecommend);

                        //由于最近一个已经处理，则从最近第2个开始计算
                        for (int tempIndex = index - 2; tempIndex > 0; tempIndex--)
                        {
                            var tempPreNoRisk = noRiskRecommendBets.FirstOrDefault(x => x.Index == tempIndex);
                            if(tempPreNoRisk == null || tempPreNoRisk.RecommendState!= preNoRiskRecommend.RecommendState|| tempPreNoRisk.RecommendTemplateName!= preNoRiskRecommend.RecommendTemplateName)
                            {
                                break;
                            }
                            tempNearNoRiskRecommends.Add(tempPreNoRisk);
                        }

                        //计算连续结果之内下注的次数
                        var tempCount = tempNearNoRiskRecommends.Count(x => riskRecommendBets.Any(z => z.Index == x.Index));
                        if(tempCount<reTryTimes)
                        {
                            isRiskBet = true;
                        }
                    }
                }

                #endregion
                if(isRiskBet)
                {
                    var current = resultEntities.FirstOrDefault(x => x.Index == index);
                    riskRecommendBets.Add(new RecommendBetModel()
                    {
                        Index = current.Index,
                        RecommendState = recommendState,
                        RecommendTemplateName = recommendName,
                        ResultState = current.ResultState
                    });
                }
                #endregion
            }
            return riskRecommendBets;
        }

        ///// <summary>
        ///// 统计预测结果
        ///// </summary>
        ///// <param name="resultEntities"></param>
        ///// <param name="recommendTemplates"></param>
        ///// <returns></returns>
        //public static List<RecommendBetModel> StatisticalProbability(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates, int reTryTimes = 2)
        //{
        //    List<RecommendBetModel> recommendBets = new List<RecommendBetModel>();

        //    List<ResultEntity> tempList;

        //    var maxIndex = resultEntities.Max(x => x.Index);
        //    bool recommend = false;
        //    ResultState betState;//推荐结果
        //    ResultState resultState;//实际结果
        //    string recommendTemplateName;
        //    for (int i = 1; i <= maxIndex; i++)
        //    {
        //        var current = resultEntities.FirstOrDefault(x => x.Index == i);
        //        if (current == null)
        //            continue;

        //        recommend = false;
        //        resultState = current.ResultState;
        //        betState = ResultState.UnKnown;
        //        recommendTemplateName = "";

        //        foreach (RecommendTemplateModel recommendTemplate in recommendTemplates)
        //        {
        //            tempList = resultEntities.Where(x => x.Index < i).ToList();
        //            if (tempList.IsRecommend(recommendTemplate, i - 1))
        //            {
        //                if (recommendTemplate.IsRecommendBet(out betState))
        //                {
        //                    recommendTemplateName = recommendTemplate.Template.Name;
        //                    recommend = true;
        //                    resultState = resultEntities.FirstOrDefault(x => x.Index == i).ResultState;
        //                    break;
        //                }
        //            }
        //        }

        //        if (recommend)
        //        {
        //            var historyBets = recommendBets.Where(x => x.Index >= i - reTryTimes && x.Index < i);
        //            if (historyBets.Count() < reTryTimes)
        //            {
        //                recommendBets.Add(new RecommendBetModel()
        //                {
        //                    Index = i,
        //                    RecommendState = betState,
        //                    ResultState = resultState,
        //                    RecommendTemplateName = recommendTemplateName
        //                });
        //                continue;
        //            }
        //        }
        //    }
        //    return recommendBets;
        //}

        ///// <summary>
        ///// 统计推荐结果，不考虑风险控制策略
        ///// </summary>
        ///// <param name="resultEntities"></param>
        ///// <param name="recommendTemplates"></param>
        ///// <returns></returns>
        //private static List<RecommendItem> StatisticalRecommend(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates)
        //{
        //    //推荐号码，不管风险控制策略
        //    List<RecommendItem> recommends = new List<RecommendItem>();

        //    List<ResultEntity> tempList;

        //    var maxIndex = resultEntities.Max(x => x.Index);
        //    bool recommend = false;
        //    ResultState betState;//推荐结果
        //    ResultState resultState;//实际结果
        //    string recommendTemplateName;
        //    for (int i = 1; i <= maxIndex; i++)
        //    {
        //        var current = resultEntities.FirstOrDefault(x => x.Index == i);
        //        if (current == null)
        //            continue;

        //        recommend = false;
        //        resultState = current.ResultState;
        //        betState = ResultState.UnKnown;
        //        recommendTemplateName = "";

        //        foreach (RecommendTemplateModel recommendTemplate in recommendTemplates)
        //        {
        //            tempList = resultEntities.Where(x => x.Index < i).ToList();
        //            if (tempList.IsRecommend(recommendTemplate, i - 1))
        //            {
        //                if (recommendTemplate.IsRecommendBet(out betState))
        //                {
        //                    recommendTemplateName = recommendTemplate.Template.Name;
        //                    recommend = true;
        //                    resultState = resultEntities.FirstOrDefault(x => x.Index == i).ResultState;
        //                    break;
        //                }
        //            }
        //        }

        //        if (recommend)
        //        {
        //            recommends.Add(new RecommendBetModel()
        //            {
        //                Index = i,
        //                RecommendState = betState,
        //                RecommendTemplateName = recommendTemplateName
        //            });
        //            continue;
        //        }
        //    }
        //    return recommends;
        //}

        //public static List<RecommendBetModel>  StatisticalRecommendBet(this List<ResultEntity> resultEntities, List<RecommendTemplateModel> recommendTemplates,int reTryTimes =2)
        //{
        //    List<RecommendBetModel> recommendBets = new List<RecommendBetModel>();

        //    //推荐结果，不考虑风险控制
        //    var recommends = resultEntities.StatisticalRecommend(recommendTemplates).OrderBy(x=>x.Index);

        //    //根据推荐结果和风险控制，重新计算推荐
        //    foreach (var currentRecommend in recommends)
        //    {
        //        bool isRecommend = false;
        //        var preRecommend = recommends.FirstOrDefault(x => x.Index == currentRecommend.Index - 1);
        //        if (preRecommend==null)//上一个推荐没有结果，则直接推荐
        //        {
        //            isRecommend = true;
        //        }
        //        //上一个推荐和本次推荐规则一致，则判断下注次数是否超过 reTryTimes
        //        else if (preRecommend.RecommendState == currentRecommend.RecommendState && preRecommend.RecommendTemplateName == currentRecommend.RecommendTemplateName)
        //        {
        //            var historyRecommends = new List<RecommendItem>();
        //            historyRecommends.Add(preRecommend);
        //            for (int i = preRecommend.Index-1; i>0 ; i--)
        //            {
        //                var tempRecommend = recommends.FirstOrDefault(x => x.Index == i);
        //                if (tempRecommend == null)
        //                {
        //                    break;
        //                }
        //                if(tempRecommend.RecommendState!=currentRecommend.RecommendState || tempRecommend.RecommendTemplateName!=currentRecommend.RecommendTemplateName)
        //                {
        //                    break;
        //                }
        //                historyRecommends.Add(tempRecommend);
        //            }
        //            var totalReTryTimes = recommendBets.Where(x => historyRecommends.Any(z => z.Index == x.Index)).Count();
        //            if(totalReTryTimes<=reTryTimes)
        //            {
        //                isRecommend = true;
        //            }
        //        }
        //        else//上一推荐和本次推荐不一致，则直接推荐
        //        {
        //            isRecommend = true;
        //        }

        //        if(isRecommend)
        //        {
        //            var res = resultEntities.FirstOrDefault(x => x.Index == currentRecommend.Index);
        //            recommendBets.Add(new RecommendBetModel()
        //            {
        //                Index = currentRecommend.Index,
        //                RecommendState = currentRecommend.RecommendState,
        //                RecommendTemplateName = currentRecommend.RecommendTemplateName,
        //                ResultState = res.ResultState
        //            });
        //        }
        //    }

        //    return recommendBets;
        //}
    }
}
