using System;

namespace Bbin.Core.Eventargs
{
    public class DealingResultEventArgs : EventArgs
    {
        public string RoomId { get; set; }
        public string Pk { get; set; }
        public string Rn { get; set; }
        public string Rs { get; set; }
    }
}
