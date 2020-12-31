using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Api.Enums
{
    /// <summary>
    /// 百家乐结果状态
    /// </summary>
    public enum ResultState
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown = 0,
        /// <summary>
        /// 闲家
        /// </summary>
        XianJia = 1,
        /// <summary>
        /// 打和
        /// </summary>
        He = 2,
        /// <summary>
        /// 庄家
        /// </summary>
        ZhuangJia = 3
    }
}
