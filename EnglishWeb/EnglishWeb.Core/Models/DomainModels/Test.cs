using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class Test : Entity
    {
        [MinLength(2)]
        public string Name { get; set; }

        public virtual List<Question> Questions { get; set; }

        public TestType Type { get; set; }

        public virtual User User { get; set; }

        public Guid UserId { get; set; }
    }
}
