using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromotionApi.Data;

namespace PromotionApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<DatabaseContext>(options =>
                   options.UseInMemoryDatabase("TempDb"));
            }
            else
            {
                services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(
                        options => options.UseNpgsql(
                            Configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DatabaseContext context)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseHttpsRedirection();
                context.Database.EnsureCreated();
            }
            else
            {
                app.UseHsts();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            //app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
