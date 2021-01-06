using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Api.Models
{
    public class QueueModel<T>
    {
        public QueueModel() {
            this.Sn = DateTime.Now.ToFileTime();
        }
        public QueueModel(string key, T data):this()
        {
            this.Key = key;
            this.Data = data;
        }

        public QueueModel(long sn,string key, T data)
        {
            this.Sn = sn;
            this.Key = key;
            this.Data = data;
        }

        public long Sn { get; set; }
        public string Key { get; set; }
        public T Data { get; set; }
    }
}
