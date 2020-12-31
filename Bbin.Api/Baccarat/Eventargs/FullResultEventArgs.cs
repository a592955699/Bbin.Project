using Bbin.Api.Baccarat.Configs;
using System;

namespace Bbin.Api.Baccarat.Eventargs
{
    public class FullResultEventArgs : EventArgs
    {
        public Round Round { get; set; }
    }
}
