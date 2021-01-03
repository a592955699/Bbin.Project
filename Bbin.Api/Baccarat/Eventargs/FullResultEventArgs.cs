using Bbin.Api.Baccarat.Entitys;
using System;

namespace Bbin.Api.Baccarat.Eventargs
{
    public class FullResultEventArgs : EventArgs
    {
        public RoundModel Round { get; set; }
    }
}
