using Bbin.Sniffer;
using System.Collections.Generic;

namespace Bbin.Sniffer.Actions
{
    public class OnHallLoginAction : IActionExecutor
    {
        public void ExecuteAsync(Dictionary<string, object> data, ISocketService webSocketWrap, params object[] paras)
        {
            //var sendData = @"{""dev"":{""rd"":""fx"",""ua"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36"",""os"":""Windows 10"",""srs"":""1680x1050"",""wrs"":""1440x900"",""dpr"":1,""pl"":""H5"",""wv"":""false"",""aio"":false,""vga"":""ANGLE (AMD Radeon HD 7660D Direct3D11 vs_5_0 ps_5_0)"",""tablet"":false,""cts"":1564808138380,""mua"":"""",""dtp"":"""",""ui"":8},""lang"":""cn"",""vType"":""wss"",""vtMode"":true,""subscription"":[""shit""],""action"":""login"",""sid"":""" + webSocketWrap.SessionId + @"""}";

            //国际厅
            var sendData = "{\"dev\":{\"rd\":\"fx\",\"ua\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36\",\"os\":\"Windows 10\",\"srs\":\"1680x1050\",\"wrs\":\"1440x900\",\"dpr\":1,\"pl\":\"H5\",\"pf\":\"Chrome 84.0.4147.89\",\"wv\":\"false\",\"aio\":false,\"vga\":\"ANGLE (AMD Radeon HD7660DDirect3D11vs_5_0ps_5_0)\",\"tablet\":false,\"cts\":1595727825326,\"mua\":\"\",\"dtp\":\"\",\"ui\":2},\"lang\":\"cn\",\"vType\":\"wss\",\"vtMode\":true,\"subscription\":[\"shit\"],\"action\":\"login\",\"sid\":\"" + webSocketWrap.SessionId + "\"}";
            webSocketWrap.Send(sendData);
        }
        public bool SetParams<T>(string name, T param, params object[] paras)
        {
            return false;
        }
    }
}
