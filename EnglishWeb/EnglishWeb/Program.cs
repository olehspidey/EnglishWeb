using System.Threading.Tasks;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.DAL.Initializers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using IdentityRole = EnglishWeb.Core.Models.DomainModels.IdentityRole;

namespace EnglishWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await RoleInitializer.InitAsync(services.GetRequiredService<RoleManager<IdentityRole>>());
                await UserInitializer.InitAsync(services.GetRequiredService<UserManager<User>>());
            }

            await webHost.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
