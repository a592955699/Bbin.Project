using Bbin.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Manager.Rate
{
    public class RateRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<RecommendTemplateModel> RecommendTemplateModels;
    }
}
