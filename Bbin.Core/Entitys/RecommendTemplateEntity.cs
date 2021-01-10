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
        public string Name { get; set; }
        public string Describe { get; set; }
        public bool Publish { get; set; }
    }
}
