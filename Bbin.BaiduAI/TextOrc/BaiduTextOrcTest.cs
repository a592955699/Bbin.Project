using Baidu.Aip.Ocr;
using Bbin.BaiduAI.Config;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bbin.BaiduAI.TextOrc
{
    public class BaiduTextOrcTest
    {
        public void Test()
        {
            //去配置文档读取配置
            BaiduConfig baiduConfig = new BaiduConfig()
            {
                AppId= "17009825",
                ApiKey = "HmWuhVwnDaIQ0OWC4mEs24RG",
                SecretKey = "cZc6TeGQL2eGgpkpgKtmacbQC1WUtLOZ"
            };

            string filePath = @"f:\files\verificationCode_keyword.png";
            var client = new Ocr(baiduConfig.ApiKey, baiduConfig.SecretKey);
            client.Timeout = 60000;
            var image = File.ReadAllBytes(filePath);
            var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GeneralBasic(image, options);
            Console.WriteLine(result);



            var url = "https://ai.bdstatic.com/file/03D0F32FE36C4E3A893D1AD60E797F5B";
            // 调用通用文字识别, 图片参数为远程url图片，可能会抛出网络等异常，请使用try/catch捕获
            var result2 = client.GeneralBasicUrl(url);
            Console.WriteLine(result2);
            // 如果有可选参数
            var options2 = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 带参数调用通用文字识别, 图片参数为远程url图片
            result2 = client.GeneralBasicUrl(url, options2);
            Console.WriteLine(result2);
        }
    }
}
