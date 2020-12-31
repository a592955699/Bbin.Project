using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbin.BaiduAI.TextOrc.Model
{
    [Serializable]
    public class BaiduResult
    {
        public long log_id { get; set; }
        public int direction { get; set; }
        public int words_result_num { get; set; }
        public int language { get; set; }
        public List<BaiduResult_Item> words_result { get; set; }

        public class BaiduResult_Item
        {
            public string words { get; set; }
            public Dictionary<string, object> probability { get; set; }
        }
    }
}
