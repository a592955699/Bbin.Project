using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bbin.Core.Cons;
using Microsoft.AspNetCore.Mvc;

namespace Bbin.ManagerWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(RoomCons.Rooms);
        }
    }
}
