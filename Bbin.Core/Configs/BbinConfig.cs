using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Configs
{
    public class BbinConfig
    {
        public string Url { get; set; } = "wss://liveess.com/fxLive/fxLB?gameType=h5multi2";
        /// <summary>
        /// 采集的 Room 设置
        /// </summary>
        public List<string> Rooms { get; set; } = new List<string>();
    }
}
