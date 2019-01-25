using System;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class TestAnswer : Entity
    {
        [MinLength(2)]
        public string Text { get; set; }

        public bool IsTrue { get; set; }

        public virtual Question Question { get; set; }

        public Guid QuestionId { get; set; }
    }
}
