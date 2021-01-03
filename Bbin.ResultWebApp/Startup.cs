using Bbin.Core.RabbitMQ;
using Bbin.Data;
using Bbin.Result;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bbin.ResultWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSingleton(Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>());
            services.AddSingleton<RabbitMQClient>();
            services.AddScoped<IResultDbService, ResultDbService>();
            services.AddScoped<IGameDbService, GameDbService>();
            services.AddScoped<IResultService, ResultService>();

            services.AddDbContext<BbinDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("BbinDbContext")
               , b => b.MigrationsAssembly("Bbin.ResultWebApp"))
            );

            //services.AddDbContext<BbinDbContext>(options =>
            //   options.UseMySql(Configuration.GetConnectionString("BbinDbContext")
            //   , b => b.MigrationsAssembly("Bbin.ResultWebApp")));
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
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
