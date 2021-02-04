using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bbin.Core;
using Bbin.Core.Extensions;
using Bbin.Data;
using Bbin.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Webdiyer.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bbin.ManagerWebApp.Controllers
{
    public class GameController : Controller
    {

        private readonly ILogger<GameApiController> _logger;

        private readonly ManagerApplicationContext _managerApplicationContext;

        private readonly IGameDbService _gameDbService;
        private readonly IResultDbService _resultDbService;

        public GameController(ILogger<GameApiController> logger, ManagerApplicationContext managerApplicationContext,
            IGameDbService gameDbService,
            IResultDbService resultDbService)
        {
            _logger = logger;
            _managerApplicationContext = managerApplicationContext;
            _gameDbService = gameDbService;
            _resultDbService = resultDbService;
        }

        public IActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            var list = _gameDbService.FindList(pageIndex, pageSize);

            return View(list);
        }

        public IActionResult Details(long id)
        {
            var game = _gameDbService.FindById(id);
            if (game == null) return null;
            var results = _resultDbService.FindList(id);
            var resultModel = game.ToGameResultModel(results);
            return View(resultModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameId">Game Id</param>
        /// <returns></returns>
        public IActionResult NextResult(int gameId, string roomId)
        {
            var game = _gameDbService.FindNext(gameId, roomId);
            if (game == null) return new JsonResult("找不到对应的Game");
            return RedirectToAction("Details", new { id = game.GameId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameId">Game Id</param>
        /// <returns></returns>
        public IActionResult PreResult(int gameId, string roomId)
        {
            var game = _gameDbService.FindPre(gameId, roomId);
            if (game == null) return new JsonResult("找不到对应的Game");
            return RedirectToAction("Details", new { id= game.GameId });
        }

        public IActionResult StatisticalRate(int gameId)
        {
            var game = _gameDbService.FindById(gameId);
            if (game == null) return new JsonResult(null);

            var results = _resultDbService.FindList(gameId);

            var managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();
            var recommendBets = results.StatisticalProbability(managerApplicationContext.RecommendTemplateModels);
            var total = recommendBets.Count();
            var win = recommendBets.Where(x => x.RecommendState == x.ResultState).Count();
            var he = recommendBets.Where(x => x.RecommendState != x.ResultState && x.ResultState == Core.Enums.ResultState.He).Count();
            var lose = recommendBets.Where(x => x.RecommendState != x.ResultState && x.ResultState != Core.Enums.ResultState.He).Count();
            double rate = 0;
            if (total != 0)
                rate = win / (double)total * 100;
            return new JsonResult(new { Detail = recommendBets, Total = total, Win = win, Lose = lose, He = he, Rate = rate.ToString("f2") });
        }
    }
}
