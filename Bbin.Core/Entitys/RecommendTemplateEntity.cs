using Bbin.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bbin.Core.Entitys
{
    /// <summary>
    /// 好路推荐模板
    /// </summary>
    public class RecommendTemplateEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 发布状态
        /// </summary>
        public bool Publish { get; set; }
        /// <summary>
        /// 下注推荐规则
        /// </summary>
        public RecommendTypeEnum RecommendType { get; set; }
    }
}
