using Bbin.Core;
using Bbin.Core.Configs;
using Bbin.Data;
using Bbin.Manager;
using Bbin.Manager.ActionExecutors;
using Bbin.ManagerWebApp.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bbin.ManagerWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ApplicationContext.Configuration = Configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Demo", Version = "v1" });
            });

            services.AddControllersWithViews();

            services.AddSignalR();

            services.AddSingleton(Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
            services.AddSingleton<IMQService, RabbitMQService>();
            services.AddSingleton<ManagerApplicationContext>();
            services.AddScoped<PublishSnifferUpActionExecutor>();
            services.AddScoped<PublishResultActionExcutor>();

//#if DEBUG
//            services.AddDbContext<BbinDbContext>(options =>
//               options.UseSqlServer(Configuration.GetConnectionString("BbinDbContext")
//               , b => b.MigrationsAssembly("Bbin.ResultConsoleApp"))
//            );
//#else
            services.AddDbContext<BbinDbContext>(options => 
                options.UseMySQL(Configuration.GetConnectionString("BbinDbContext")
                   , b => b.MigrationsAssembly("Bbin.ResultConsoleApp")));
//#endif

            services.AddScoped<IResultDbService, ResultDbService>();
            services.AddScoped<IGameDbService, GameDbService>();
            services.AddScoped<IRecommendItemService, RecommendItemService>();
            services.AddScoped<IRecommendTemplateService, RecommendTemplateService>();

            
            //全局配置Json序列化处理
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            ApplicationContext.ServiceProvider = services.BuildServiceProvider();
        }
       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // 添加Swagger有关中间件
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //默认路由
                endpoints.MapControllerRoute(
                    name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

                //区域路由(要放在默认路由的后面)
                //注：必须以特性的形式在对应控制器上加上区域名称 [Area("XXXX")]
                endpoints.MapControllerRoute(
                   name: "default2",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<GameHub>("/gameHub");
            });

            //ApplicationContext.ServiceProvider = app.ApplicationServices;
        }
    }
}
