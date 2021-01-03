using System;
using Newtonsoft.Json;

namespace Bbin.Api.Baccarat.Entitys
{
    public class RoundModel
    {
        [JsonProperty("room")]
        public string RoomId { get; set; }
        [JsonProperty("cd")]
        public string Cd { get; set; }
        [JsonProperty("map")]
        public string Map { get; set; }
        [JsonProperty("pKey")]
        public string PKey { get; set; }
        [JsonProperty("pk")]
        public string Pk { get; set; }
        [JsonProperty("rn")]
        public string Rn { get; set; }
        [JsonProperty("rs")]
        public string Rs { get; set; }
        /// <summary>
        /// waiting 开结果;
        /// betting 可以下注，只有这个时候有 rn rs;
        /// dealing 发牌
        /// </summary>
        [JsonProperty("st")]
        public string St { get; set; }
        [JsonProperty("dn")]
        public string Dn { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}
