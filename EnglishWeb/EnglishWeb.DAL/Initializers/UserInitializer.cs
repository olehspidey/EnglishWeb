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
            var superAdmin = CreateSuperAdmin();
            var teacher = CreateTeacher();
            const string password = "30Test30";

            await userManager.CreateAsync(superAdmin, password);
            await userManager.CreateAsync(teacher, password);
            await userManager.AddToRoleAsync(superAdmin, UserRoles.Admin);
            await userManager.AddToRoleAsync(superAdmin, UserRoles.Teacher);
            await userManager.AddToRoleAsync(teacher, UserRoles.Teacher);
        }

        private static User CreateSuperAdmin()
            => new User
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@gmail.com",
                UserName = "admin",
                EmailConfirmed = true,
                IsActive = true
            };

        private static User CreateTeacher()
            => new User
            {
                Name = "Teacher",
                LastName = "Teacher",
                Email = "teacher@gmail.com",
                UserName = "teacher",
                EmailConfirmed = true,
                IsActive = true
            };
    }
}
