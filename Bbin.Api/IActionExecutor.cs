using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Api
{
    /// <summary>
    /// 执行器接口
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public interface IActionExecutor
    {
        object DoExcute(object args);
    }
}
