using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bbin.ManagerWebApp.Models;
using Bbin.Manager;
using Bbin.Core.Models;
using Bbin.Core.Cons;
using Bbin.Core.Commandargs;
using Bbin.Manager.ActionExecutors;
using Bbin.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bbin.ManagerWebApp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;



        public ManagerController(ILogger<ManagerController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 采集账号列表
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var _managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();
            return View(_managerApplicationContext.Sniffers.OrderBy(x => x.QueueName));
        }
        [HttpGet]
        public IActionResult Start(string queueName)
        {
            var _managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();
            var sniffer = _managerApplicationContext.GetSniffer(queueName);
            return View(sniffer);
        }
        [HttpPost]
        public IActionResult Start(SnifferUpArgs sniffer)
        {
            //发动采集申请
            var newQueueModel = new QueueModel<SnifferUpArgs>(CommandKeys.PublishSnifferStart, sniffer);
            RabbitMQUtils.SendMessage(sniffer.QueueName, newQueueModel);
            ViewBag.Message = "已发送 SnifferStart 命令，等待10秒后将更新状态";
            return View(sniffer);
        }
        /// <summary>
        /// 启动 snffer 采集
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public IActionResult SnifferStart(string queueName)
        {
            var _managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();
            var sniffer = _managerApplicationContext.GetSniffer(queueName);

            //发动采集申请
            var newQueueModel = new QueueModel<SnifferUpArgs>(CommandKeys.PublishSnifferStart, sniffer);
            RabbitMQUtils.SendMessage(sniffer.QueueName, newQueueModel);

            return Content("已发送 SnifferStart 命令");
        }

        /// <summary>
        /// 停止 sniffer 采集
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public IActionResult SnifferStop(string queueName)
        {
            var _managerApplicationContext = ApplicationContext.ServiceProvider.GetService<ManagerApplicationContext>();
            var sniffer = _managerApplicationContext.GetSniffer(queueName);

            //发动采集申请
            var newQueueModel = new QueueModel<SnifferStopArgs>(CommandKeys.PublishSnifferStop, null);
            RabbitMQUtils.SendMessage(sniffer.QueueName, newQueueModel);

            return Content("已发送 SnifferStop 命令");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
