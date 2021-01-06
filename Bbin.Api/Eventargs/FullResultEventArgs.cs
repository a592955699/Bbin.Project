using Bbin.Api.Model;
using System;

namespace Bbin.Api.Eventargs
{
    public class FullResultEventArgs : EventArgs
    {
        public RoundModel Round { get; set; }
    }
}
