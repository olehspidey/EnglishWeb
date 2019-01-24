using System.Threading.Tasks;
using EnglishWeb.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityRole = EnglishWeb.Core.Models.DomainModels.IdentityRole;

namespace EnglishWeb.DAL.Initializers
{
    public static class RoleInitializer
    {
        public static async Task InitAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Teacher));
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
        }
    }
}
