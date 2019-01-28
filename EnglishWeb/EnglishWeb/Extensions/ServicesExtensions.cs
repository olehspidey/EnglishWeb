using EnglishWeb.BLL.Services;
using EnglishWeb.BLL.Services.Abstract;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Data;
using EnglishWeb.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EnglishWeb.Extensions
{
    internal static class ServicesExtensions
    {
        public static void AddDatabaseContext(this IServiceCollection services, IHostingEnvironment environment, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(environment.IsDevelopment() ? "DefaultConnection" : "ProductionConnection")));
        }

        public static void AddOwnServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEmailSendingService, EmailSendingService>();
        }

        public static void AddIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<User, Core.Models.DomainModels.IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>()
                .AddRoleManager<RoleManager<Core.Models.DomainModels.IdentityRole>>()
                .AddDefaultTokenProviders();
        }
    }
}
