using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.BaiduAI.ImageOrc.Model
{
    /// <summary>
    /// 图像识别结果model
    /// </summary>
    [Serializable]
    public class AdvancedGeneralResult
    {
        public string log_id { get; set; }
        public int result_num { get; set; }
        public List<resultItem> result { get; set; } = new List<resultItem>();
        public class resultItem
        {
            public decimal score { get; set; }
            public string root { get; set; }
            public string keyword { get; set; }
        }
    }
}
