//#define TEMP_DB

using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromotionApi.Data;
using PromotionApi.Models;

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

#if TEMP_DB
            services.AddDbContext<DatabaseContext>(options =>
                   options.UseInMemoryDatabase("TempDb"));
#else
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
#endif

            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();
            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DatabaseContext context)
        {
            app.UseIpRateLimiting();

#if TEMP_DB
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseHttpsRedirection();
                context.Database.EnsureCreated();
            }
            else
            {
                context.Database.EnsureCreated();
                app.UseHsts();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
#else
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseHttpsRedirection();
                context.Database.EnsureCreated();
            }
            else
            {
                app.ConfigureExceptionHandler();
                app.UseHsts();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
#endif

            //app.UseStaticFiles();

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
            );

            app.UseMvc();
        }
    }
}
