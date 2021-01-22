using Bbin.Core.Entitys;
using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Models.UI
{
    public class PushGameResultModel: GameResultModel
    {
        /// <summary>
        /// 推荐下注
        /// </summary>
        public Dictionary<RecommendTemplateEntity, ResultState> Recommend { get; set; }
    }
}
