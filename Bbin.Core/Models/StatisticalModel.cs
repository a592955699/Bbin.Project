using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Models
{
    public class StatisticalModel
    {
        /// <summary>
        /// 推荐下注次数
        /// </summary>
        public int BetTimes { get; set; }
        /// <summary>
        /// 正确次数
        /// </summary>
        public int WinTimes { get; set; }
        /// <summary>
        /// 出现和次数
        /// </summary>
        public int HeTimes { get; set; }
    }
}
