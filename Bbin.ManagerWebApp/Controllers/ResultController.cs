using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bbin.Core.Extensions;
using Bbin.Core.Models.UI;
using Bbin.Data;
using Bbin.Manager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bbin.ManagerWebApp.Controllers
{
    public class ResultController : Controller
    {
        private readonly ILogger<ResultController> _logger;

        private readonly ManagerApplicationContext _managerApplicationContext;

        private readonly IGameDbService _gameDbService;
        private readonly IResultDbService _resultDbService;

        public ResultController(ILogger<ResultController> logger, ManagerApplicationContext managerApplicationContext,
            IGameDbService gameDbService,
            IResultDbService resultDbService)
        {
            _logger = logger;
            _managerApplicationContext = managerApplicationContext;
            _gameDbService = gameDbService;
            _resultDbService = resultDbService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <returns></returns>
        public IActionResult Result(int id)
        {
            var game = _gameDbService.FindById(id);
            if (game == null) return null;
            var results = _resultDbService.FindList(id);
            var resultModel = game.ToGameResultModel(results);
            return new JsonResult(resultModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <returns></returns>
        public IActionResult NextResult(int id)
        {
            var game = _gameDbService.FindNext(id);
            if (game == null) return new JsonResult(null); ;
            var results = _resultDbService.FindList(id);
            var resultModel = game.ToGameResultModel(results);
            return new JsonResult(resultModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <returns></returns>
        public IActionResult PreResult(int id)
        {
            var game = _gameDbService.FindPre(id);
            if (game == null) return new JsonResult(null);
            var results = _resultDbService.FindList(id);
            var resultModel = game.ToGameResultModel(results);
            return new JsonResult(resultModel);
        }
    }
}
