using Bbin.Core.Entitys;
using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Models.UI
{
    public class PushGameResultModel : GameResultModel
    {
        /// <summary>
        /// 推荐下注
        /// </summary>
        public List<RecommendResultItem> Recommend { get; set; }


        public class RecommendResultItem
        {
            public int Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            public ResultState ResultState { get; set; }
            /// <summary>
            /// 下注推荐规则
            /// </summary>
            public RecommendTypeEnum RecommendType { get; set; }
        }
    }
}
