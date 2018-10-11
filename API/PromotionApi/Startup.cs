using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromotionApi.Data;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

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
                services.AddDbContext<DatabaseContext>(options => options.ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning)).UseInMemoryDatabase("TempDb"));
            }
            else
            {
                services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(options => options.ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning)).UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            }

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

            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.SuppressModelStateInvalidFilter = true;
            });

            services
                .AddMvcCore(x => {
                    x.Filters.Add<ApiValidateModelAttribute>();
                })
                .AddApiExplorer()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddCacheTagHelper()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddCors()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Promotion Api",
                    Version = "v1",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DatabaseContext context)
        {
            app.UseIpRateLimiting();

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

            app.ConfigureExceptionHandler();

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
            );

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Promotion Api v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
