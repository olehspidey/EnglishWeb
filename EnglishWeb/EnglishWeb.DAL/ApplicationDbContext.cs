using System;
using EnglishWeb.Core.Models.DomainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnglishWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
