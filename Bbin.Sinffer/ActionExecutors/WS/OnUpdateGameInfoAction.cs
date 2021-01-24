using Bbin.Sniffer;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Bbin.Core.Entitys;
using Bbin.Sniffer.Cons;
using Bbin.Core.Cons;
using Bbin.Core;
using Microsoft.Extensions.Configuration;
using Bbin.Core.Model;
using Bbin.Core.Configs;
using Bbin.Sniffer.SnifferActionExecutors;

namespace Bbin.SnifferActionExecutors
{
    public class OnUpdateGameInfoAction : AbstractWsActionExecutor
    {
        /// <summary>
        /// 过滤采集的牌桌
        /// </summary>
        public List<string> TableMap { get; private set; } = new List<string>();
        Dictionary<string, RoundModel> RnRsMap { get; set; } = new Dictionary<string, RoundModel>();

        public OnUpdateGameInfoAction()
        {
            var bbinConfig = ApplicationContext.Configuration.GetSection("BbinConfig").Get<BbinConfig>();
            if (bbinConfig != null && bbinConfig.Rooms != null)
            {
                TableMap = bbinConfig.Rooms;
            }
        }
        public override void Execute(params object[] paras)
        {
            var sl = (JObject)Data["sl"];
            var slDict = sl.ToObject<Dictionary<string, Dictionary<string, object>>>();

            List<RoundModel> rooms = new List<RoundModel>();

            foreach (KeyValuePair<string, Dictionary<string, object>> keyValue in slDict)
            {
                if (TableMap.Count != 0 && !TableMap.Contains(keyValue.Key))
                    continue;

                RoundModel round = new RoundModel();

                round.RoomId = keyValue.Key;
                object temp;
                if (keyValue.Value.TryGetValue("cd", out temp))
                    round.Cd = temp.ToString();

                if (keyValue.Value.TryGetValue("pKey", out temp))
                    round.PKey = temp.ToString();

                if (keyValue.Value.TryGetValue("pk", out temp))
                    round.Pk = temp.ToString();

                if (keyValue.Value.TryGetValue("st", out temp))
                    round.St = temp.ToString();

                if (round.St == "betting")
                {
                    if (keyValue.Value.TryGetValue("rn", out temp))
                        round.Rn = temp.ToString();

                    if (keyValue.Value.TryGetValue("rs", out temp))
                        round.Rs = temp.ToString();
                    round.Begin = DateTime.Now;
                    //记录 rn rs 供后续使用
                    //rn rs 只有在第一次的时候才有
                    SetRound(round);
                }
                else
                {
                    var tempRound = GetRound(round.RoomId);
                    if (tempRound != null)
                    {
                        round.Rn = tempRound.Rn;
                        round.Rs = tempRound.Rs;
                        round.Begin = tempRound.Begin;
                        round.End = DateTime.Now;
                    }
                }

                if (keyValue.Value.TryGetValue("map", out temp))
                    round.Map = temp.ToString();

                if (keyValue.Value.TryGetValue("dn", out temp))
                    round.Dn = temp.ToString();
                rooms.Add(round);
            }

            foreach (var item in rooms)
            {
                if (!String.IsNullOrWhiteSpace(item.St))
                {
                    SocketService.InternalOnStateChange(item);
                }

                if (item.St == "waiting")
                {
                    SocketService.InternalOnFullResult(item);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(item.Cd))
                {
                    SocketService.InternalOnCd(item);
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(item.Pk))
                {
                    SocketService.InternalOnDealingResult(item);
                    continue;
                }
                log.Debug(DateTime.Now.ToString("HH:mm:ss") + " " + JsonConvert.SerializeObject(item));
            }
        }
        void SetRound(RoundModel round)
        {
            RnRsMap[round.RoomId] = round;
        }
        RoundModel GetRound(string roundId)
        {
            RoundModel round;
            if (RnRsMap.TryGetValue(roundId, out round) && (DateTime.Now - round.Begin).TotalSeconds <= BbinCons.TowRoundInterval)
            {
                return round;
            }
            else
            {
                return null;
            }
        }
        public override void SetParams<T>(string name, T param)
        {
            if (name == "TableMap")
            {
                TableMap.Clear();
                RnRsMap.Clear();
                foreach (var item in (param as List<string>))
                {
                    TableMap.Add(item);
                    log.Debug($"重置过滤 {item}");
                }
            }
        }
    }
}
