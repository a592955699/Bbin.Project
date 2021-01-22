using Bbin.Sniffer;
using Bbin.Sniffer.SnifferActionExecutors;
using System.Collections.Generic;

namespace Bbin.SnifferActionExecutors
{
    public class ReadyAction : AbstractWsActionExecutor
    {
        public override object DoExecute(params object[] paras)
        {
            //var sendData = @"{""dev"":{""rd"":""fx"",""ua"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36"",""os"":""Windows 10"",""srs"":""1680x1050"",""wrs"":""1440x900"",""dpr"":1,""pl"":""H5"",""wv"":""false"",""aio"":false,""vga"":""ANGLE (AMD Radeon HD 7660D Direct3D11 vs_5_0 ps_5_0)"",""tablet"":false,""cts"":1564808138312,""mua"":"""",""dtp"":"""",""ui"":8},""lang"":""cn"",""vType"":""wss"",""sid"":""" + webSocketManager.Sid + @""",""action"":""hallLogin""}"; ;
            var sendData = "{\"lang\":\"cn\",\"vType\":\"wss\",\"site\":1,\"sid\":\"" + SocketService.SessionId + "\",\"action\":\"hallLogin\"}";
            SocketService.Send(sendData);
            return null;
        }
    }
}
