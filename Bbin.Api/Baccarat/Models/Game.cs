using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bbin.Api.Baccarat.Configs
{
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GameId { get; set; }
        /// <summary>
        /// 牌桌号
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// 第几靴
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 每一靴的开始日期 yyyyMMdd格式，冗余主要做查询用（从中间开始采集的时候，可能不准确）
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 采集到的的第一局的时间
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
