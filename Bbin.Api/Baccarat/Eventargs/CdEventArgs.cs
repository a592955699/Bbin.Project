using System;

namespace Bbin.Api.Baccarat.Eventargs
{
    public class CdEventArgs : EventArgs
    {
        public string RoomId { get; set; }
        public string Cd { get; set; }
        public string Rn { get; set; }
        public string Rs { get; set; }
    }
}
