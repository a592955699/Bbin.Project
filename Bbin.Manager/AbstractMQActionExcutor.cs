//using Bbin.Core;
//using Bbin.ManagerWebApp.Hubs;
//using System;
//using Microsoft.Extensions.DependencyInjection;
//using System.Threading.Tasks;

//namespace Bbin.Manager.ActionExecutors
//{
//    public abstract class AbstractMQActionExcutor : IActionExecutor
//    {
//        public GameHub GameHub { get; private set; } = ApplicationContext.ServiceProvider.GetService<GameHub>();
//        public abstract object DoExecute(params object[] args);
//    }
//}
