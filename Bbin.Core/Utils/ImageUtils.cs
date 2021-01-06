using Bbin.Api.Cons;
using log4net;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Bbin.Core.Utils
{
    public class ImageUtils
    {
        static ILog log = LogManager.GetLogger(Log4NetCons.LoggerRepositoryName, typeof(ImageUtils));
        public static bool CutSmallPng(string path, string smallpath)
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                int firstBmpHeight = 150;
                int secondBmpHeight = 300;
                if (bmp.Height < firstBmpHeight) return false;
                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                #region 从上往下找缺高度
                bool flag = true;
                for (int i = firstBmpHeight + 5; i < secondBmpHeight - 5; i++)
                {
                    y1 = i;
                    flag = true;
                    for (int j = 2; j < bmp.Width; j++)
                    {
                        var color = bmp.GetPixel(j, i);
                        if (!IsBackGround(color))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                #endregion

                #region 从下往上找缺口高度
                for (int i = secondBmpHeight - 5; i >= firstBmpHeight + 5; i--)
                {
                    y2 = i;
                    flag = true;
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        var color = bmp.GetPixel(j, i);
                        if (!IsBackGround(color))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                #endregion

                #region 从左往右
                for (int i = 0; i < bmp.Width; i++)
                {
                    x1 = i;
                    flag = true;
                    for (int j = firstBmpHeight + 5; j < secondBmpHeight - 5; j++)
                    {
                        var color = bmp.GetPixel(i, j);
                        if (!IsBackGround(color))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                #endregion

                #region 从右往左
                for (int i = bmp.Width - 1; i >= 0; i--)
                {
                    x2 = i;
                    flag = true;
                    for (int j = firstBmpHeight + 5; j < secondBmpHeight - 5; j++)
                    {
                        var color = bmp.GetPixel(i, j);
                        if (!IsBackGround(color))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                #endregion

                #region 去除黑色底色位置
                //创建新图位图
                //Bitmap bitmap = new Bitmap(24, 34);
                using (Bitmap bitmap = new Bitmap(20, 28))
                {
                    //创建作图区域
                    using (Graphics graphic = Graphics.FromImage(bitmap))
                    {
                        //截取原图相应区域写入作图区
                        graphic.DrawImage(bmp, 0, 0, new Rectangle(x1 + 12, y1 + 14, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);

                        log.Debug($"x1:{x1 + 12} y1:{y1 + 14} w:{bitmap.Width} h:{bitmap.Height}");

                        //从作图区生成新图
                        Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
                        //保存图片
                        saveImage.Save(smallpath, ImageFormat.Png);
                        //释放资源   
                        saveImage.Dispose();
                        graphic.Dispose();
                        bitmap.Dispose();
                    }
                }
                #endregion
                bmp.Dispose();
            }

            return true;
        }

        /// <summary>
        /// 从图片中截取一部分图片
        /// </summary>
        /// <param name="fromImagePath">来源图片地址</param>        
        /// <param name="nX">从偏移X坐标位置开始截取</param>
        /// <param name="nY">从偏移Y坐标位置开始截取</param>
        /// <param name="toImagePath">保存图片地址</param>
        /// <param name="width">保存图片的宽度</param>
        /// <param name="height">保存图片的高度</param>
        /// <returns></returns>
        public static void CaptureImage(string fromImagePath, int nX, int nY, string toImagePath, int width, int height)
        {
            //原图片文件
            using (Image fromImage = Image.FromFile(fromImagePath))
            {
                //创建新图位图
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    //创建作图区域
                    using (Graphics graphic = Graphics.FromImage(bitmap))
                    {
                        //截取原图相应区域写入作图区
                        graphic.DrawImage(fromImage, 0, 0, new Rectangle(nX, nY, width, height), GraphicsUnit.Pixel);
                        //从作图区生成新图
                        Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
                        //保存图片
                        saveImage.Save(toImagePath, ImageFormat.Png);
                        //释放资源   
                        saveImage.Dispose();
                        graphic.Dispose();
                        bitmap.Dispose();
                        fromImage.Dispose();
                    }
                }
            }
        }

        private static bool IsBackGround(Color color)
        {
            int roncha = 20;

            var bgColor = Color.FromArgb(255, 0, 0, 0);

            return Math.Abs(color.A - bgColor.A) <= roncha
                && Math.Abs(color.R - bgColor.R) <= roncha
                && Math.Abs(color.G - bgColor.G) <= roncha
                && Math.Abs(color.B - bgColor.B) <= roncha;
        }
    }
}
