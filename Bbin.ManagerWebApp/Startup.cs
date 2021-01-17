using Bbin.Core;
using Bbin.Core.Configs;
using Bbin.Data;
using Bbin.Manager;
using Bbin.Manager.ActionExecutors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllersWithViews();

            services.AddSingleton(Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
            services.AddSingleton<IMQService, RabbitMQService>();
            services.AddSingleton<ManagerApplicationContext>();
            services.AddScoped<PublishSnifferUpActionExecutor>();
            services.AddScoped<PublishResultActionExcutor>();

#if DEBUG
            services.AddDbContext<BbinDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("BbinDbContext")
               , b => b.MigrationsAssembly("Bbin.ResultConsoleApp"))
            );
#else
            services.AddDbContext<BbinDbContext>(options => 
                options.UseMySQL(Configuration.GetConnectionString("BbinDbContext")
                   , b => b.MigrationsAssembly("Bbin.ResultConsoleApp")));
#endif

            services.AddScoped<IResultDbService, ResultDbService>();
            services.AddScoped<IGameDbService, GameDbService>();
            services.AddScoped<IRecommendItemService, RecommendItemService>();
            services.AddScoped<IRecommendTemplateService, RecommendTemplateService>();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            ApplicationContext.ServiceProvider = app.ApplicationServices;
        }
    }
}
