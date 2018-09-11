using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromotionApi.Data;

namespace PromotionApi
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
#if DEBUG
            services.AddDbContext<DatabaseContext>(options =>
               options.UseInMemoryDatabase("TempDb"));
#else
            services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(
                    options => options.UseNpgsql(
                        Configuration.GetConnectionString("DefaultConnection")));
#endif

            /*services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var identityServerBuilder = services.AddIdentityServer();
            identityServerBuilder.AddDeveloperSigningCredential();*/

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

#if DEBUG
            app.UseHttpsRedirection();
#else
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
#endif

            //app.UseStaticFiles();

            //app.UseIdentityServer();

            app.UseMvc();
        }
    }
}
