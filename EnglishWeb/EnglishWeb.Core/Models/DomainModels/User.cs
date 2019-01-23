using System;
using EnglishWeb.Core.Models.DomainModels.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
