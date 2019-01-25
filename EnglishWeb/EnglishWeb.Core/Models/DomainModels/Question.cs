using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class Question : Entity
    {
        [MinLength(2)]
        public string Name { get; set; }
        public virtual List<TestAnswer> Answers { get; set; }

        public virtual Test Test { get; set; }

        public Guid TestId { get; set; }
    }
}
