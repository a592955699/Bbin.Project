using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Models
{
    public class RecommendItem
    {
        /// <summary>
        /// 推荐数据
        /// </summary>
        public ResultState RecommendState { get; set; }
        /// <summary>
        /// 第几局
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 推荐规则名称
        /// </summary>
        public string RecommendTemplateName { get; set; }
    }
    public class RecommendBetModel : RecommendItem
    {
        /// <summary>
        /// 实际结果
        /// </summary>
        public ResultState ResultState { get; set; }
    }
}
