using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Manager.Rate
{
    public class Rate
    {
        public RateRequest RateRequest { get; set; }
        public int Total { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int He { get; set; }
        public double WinRate { get; set; }
    }
}
