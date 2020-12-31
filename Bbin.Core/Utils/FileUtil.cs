using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Bbin.Core.Utils
{
    public class FileUtil
    {
        /// <summary>
        /// 文件转 base 64 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FileToBase64(String fileName)
        {
            using (FileStream filestream = new FileStream(fileName, FileMode.Open))
            {
                byte[] arr = new byte[filestream.Length];
                filestream.Read(arr, 0, (int)filestream.Length);
                string baser64 = Convert.ToBase64String(arr);
                filestream.Close();
                return baser64;
            }
        }

        /// <summary>
        /// base64 图片内容转图片文件
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="fileName"></param>
        public static void Base64ToImageFile(string base64,string fileName)
        {
            byte[] arr = Convert.FromBase64String(base64);//将纯净资源Base64转换成等效的8位无符号整形数组
            //转换成无法调整大小的MemoryStream对象
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(arr))
            {
                //将MemoryStream对象转换成Bitmap对象
                using (var bitmap = new System.Drawing.Bitmap(ms))
                {
                    bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);//保存到服务器路径
                    ms.Close();//关闭当前流，并释放所有与之关联的资源
                    bitmap.Dispose();
                }
            }
        }
    }
}
