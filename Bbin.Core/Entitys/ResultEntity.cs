using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bbin.Core.Entitys
{
    public class ResultEntity
    {
        /// <summary>
        /// 随机主键
        /// </summary>
        [Key] //主键 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ResultId { get; set; }
        public GameEntity Game { get; set; }
        /// <summary>
        /// 一靴中的第几局
        /// 例：Rn=15-36 则 Index=36
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 冗余设计，主要为了查询使用
        /// 例：15-36
        /// </summary>
        public string Rn { get; set; }
        /// <summary>
        /// 冗余设计，主要为了查询使用
        /// 例：171858280
        /// </summary>
        public string Rs { get; set; }
        public string Card1 { get; set; }
        public string Card2 { get; set; }
        public string Card3 { get; set; }

        public string Card4 { get; set; }
        public string Card5 { get; set; }
        public string Card6 { get; set; }
        /// <summary>
        /// 最终点数
        /// </summary>
        public int Number { get; set; }
        public ResultState ResultState { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Begin { get; set; }
        /// <summary>
        /// 出结果时间
        /// </summary>
        public DateTime End { get; set; }
    }
}
