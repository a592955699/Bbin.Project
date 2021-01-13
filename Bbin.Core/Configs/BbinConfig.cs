using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Configs
{
    public class BbinConfig
    {
        public string Url { get; set; } = "wss://liveess.com/fxLive/fxLB?gameType=h5multi2";
        public Dictionary<string, string> Rooms { get; set; } = new Dictionary<string, string>();
        public string GetRoomName(string roomeId)
        {
            if(Rooms.ContainsKey(roomeId))
            {
                return Rooms[roomeId];
            }
            else
            {
                return roomeId;
            }
        }
    }
}
