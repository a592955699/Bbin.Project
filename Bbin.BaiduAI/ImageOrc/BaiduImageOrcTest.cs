using Bbin.BaiduAI.Config;
using Bbin.BaiduAI.ImageOrc.Model;
using Newtonsoft.Json;

namespace Bbin.BaiduAI.ImageOrc
{
    public class BaiduImageOrcTest
    {
        public void Test()
        {
            //去配置文档读取配置
            BaiduConfig baiduConfig = new BaiduConfig() {
                ApiKey= "GFGZBfK9NzXe7DxZKl9A2Mb3",
                SecretKey= "qmsRizCCGVr6iiaBqITZuMrjLF3tYhMk"
            };

            var token = AccessToken.getAccessToken(baiduConfig);
            var advancedGeneralResult = AdvancedGeneral.advancedGeneral(token.access_token, "f:\\files\\hcp.png");
        }
    }
}
