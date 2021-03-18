using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bbin.Core.Entitys
{
    /// <summary>
    /// 好路推荐规则项
    /// </summary>
    public class RecommendItemEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int RecommendTemplateId { get; set; }
        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultState ResultState { get; set; }
        /// <summary>
        /// 出现次数
        /// </summary>
        public int Times { get; set; } = 1;
    }
}
