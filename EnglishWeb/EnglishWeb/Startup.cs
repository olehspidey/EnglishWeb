using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EnglishWeb.Extensions;
using EnglishWeb.MapperConfig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath($"{hostingEnvironment.WebRootPath}")
                .AddJsonFile("appsettings.json")
                .AddJsonFile("emailsettings.json")
                .Build();
            Environment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(op => Configuration);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDatabaseContext(Environment, Configuration);
            services.AddOwnServices();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.LogoutPath = new PathString("/Account/LogOut");
                });

            services.AddIdentityConfig();

            services.AddAutoMapper(expression => expression.AddProfile(new MapperProfile()));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
