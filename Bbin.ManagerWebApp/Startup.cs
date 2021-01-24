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
            // ���Swagger
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

            
            //ȫ������Json���л�����
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //��ʹ���շ���ʽ��key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //����ʱ���ʽ
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

            // ���Swagger�й��м��
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Ĭ��·��
                endpoints.MapControllerRoute(
                    name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

                //����·��(Ҫ����Ĭ��·�ɵĺ���)
                //ע�����������Ե���ʽ�ڶ�Ӧ�������ϼ����������� [Area("XXXX")]
                endpoints.MapControllerRoute(
                   name: "default2",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<GameHub>("/gameHub");
            });

            //ApplicationContext.ServiceProvider = app.ApplicationServices;
        }
    }
}
