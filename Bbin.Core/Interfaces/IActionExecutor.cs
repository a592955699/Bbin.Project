using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core
{

    public interface IActionExecutor
    {
        object DoExecute(params object[] args);
    }
    public interface IActionExecutor<TIn, TOut>
    {
        TOut DoExecute(TIn args);
    }
}
