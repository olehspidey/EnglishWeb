using System.Threading.Tasks;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace EnglishWeb.DAL.Initializers
{
    public class UserInitializer
    {
        public static async Task InitAsync(UserManager<User> userManager)
        {
            var user = new User
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@gmail.com",
                UserName = "admin",
                EmailConfirmed = true,
                IsActive = true
            };

            await userManager.CreateAsync(user, "30Test30");
            await userManager.AddToRoleAsync(user, UserRoles.Admin);
        }
    }
}
