using Bbin.Api.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public interface IResultDbService
    {
        ResultEntity findById(long resultId);
        bool Insert(ResultEntity result);
        List<ResultEntity> findList(long gameId);
        ResultEntity GetResult(string roomId, string date, int gameIndex, int index);

    }
}
