using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Models.UI
{
    public class GameResultModel
    {
        public long GameId { get; set; }
        public int Index { get; set; }
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public DateTime Date{get;set;}
        /// <summary>
        /// 数字结果
        /// </summary>
        public List<NumberResultModel> NumberResults { get; set; }
        /// <summary>
        /// 颜色结果
        /// </summary>
        public List<List<ResultState>> ColumnResults { get; set; }
    }
    public class NumberResultModel { 
        public int Index { get; set; }
        /// <summary>
        /// 最终点数
        /// </summary>
        public int Number { get; set; }
        public ResultState ResultState { get; set; }
    }

}
