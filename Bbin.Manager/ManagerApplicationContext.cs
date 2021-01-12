using Bbin.Core;
using Bbin.Core.Commandargs;
using Bbin.Core.Models;
using Bbin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Bbin.Core.Extensions;

namespace Bbin.Manager
{
    /// <summary>
    /// manager 上下文
    /// #TODO 需要做成单利模式
    /// </summary>
    public class ManagerApplicationContext
    {
        public ManagerApplicationContext()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    lock (this)
                    {
                        var list = Sniffers.Where(x => (DateTime.Now - x.LastDateTime).TotalSeconds > 20).ToList();
                        foreach (var item in list)
                        {
                            Sniffers.Remove(item);
                        }
                    }
                    await Task.Delay(1000);
                }
            });
            InitRecommendTemplateModels();
        }
        /// <summary>
        /// 获取下注推荐设置
        /// </summary>
        public void InitRecommendTemplateModels()
        {
            var recommendTemplateService = ApplicationContext.ServiceProvider.GetService<IRecommendTemplateService>();
            var recommendItemService = ApplicationContext.ServiceProvider.GetService<IRecommendItemService>();
            var templates = recommendTemplateService.FindAll(true);
            var items = recommendItemService.FindAll();
            RecommendExtensions.ToRecommendTemplateModel(templates, items);
        }
        /// <summary>
        /// 好路推荐+推荐下注配置
        /// </summary>
        public List<RecommendTemplateModel> RecommendTemplateModels { get; set; }

        /// <summary>
        /// Sniffer 采集服务配置
        /// </summary>
        public List<SnifferUpArgs> Sniffers = new List<SnifferUpArgs>();

        /// <summary>
        /// sniffer 上线，添加到集合
        /// </summary>
        /// <param name="snifferUpArgs"></param>
        public void AddSniffers(SnifferUpArgs snifferUpArgs)
        {
            lock(this)
            {
                RemoveSniffers(snifferUpArgs);
                Sniffers.Add(snifferUpArgs);
            }
        }

        /// <summary>
        /// sniffer 下线，或者是心跳丢失，移除集合
        /// </summary>
        /// <param name="snifferUpArgs"></param>
        public void RemoveSniffers(SnifferUpArgs snifferUpArgs)
        {
            var sniffer = Sniffers.FirstOrDefault(x => x.QueueName == snifferUpArgs.QueueName);
            if (sniffer != null)
            {
                lock (this)
                {
                    Sniffers.Remove(sniffer);
                }
            }
        }

        public SnifferUpArgs GetSniffer(string queueName)
        {
            return Sniffers.FirstOrDefault(x => x.QueueName == queueName);
        }
    }
}
