using System;

namespace Bbin.Api.Eventargs
{
    public class StateChangeEventArgs : EventArgs
    {
        public string RoomId { get; set; }
        public string St { get; set; }
        public string Rn { get; set; }
        public string Rs { get; set; }
    }
}
