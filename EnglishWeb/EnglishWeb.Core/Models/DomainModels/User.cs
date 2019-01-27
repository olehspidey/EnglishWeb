using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [MinLength(2)]
        public string Name { get; set; }

        [MinLength(2)]
        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public virtual List<Article> Articles { get; set; }

        public virtual List<Test> Tests { get; set; }

        public virtual List<PassedTest> PassedTests { get; set; }
    }
}
