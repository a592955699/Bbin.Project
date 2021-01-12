using Bbin.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public interface IResultDbService
    {
        ResultEntity FindByRs(string rs);
        bool Insert(ResultEntity result);
        List<ResultEntity> FindList(long gameId);
        ResultEntity FindResult(string roomId, string date, int gameIndex, int index);

    }
}
