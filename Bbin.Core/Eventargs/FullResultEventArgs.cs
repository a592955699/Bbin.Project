using Bbin.Core.Model;
using System;

namespace Bbin.Core.Eventargs
{
    public class FullResultEventArgs : EventArgs
    {
        public RoundModel Round { get; set; }
    }
}
