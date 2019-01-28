using System;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class Article : Entity
    {
        [MinLength(2), Required]
        public string Name { get; set; }

        [MinLength(10), Required]
        public string Text { get; set; }

        public ArticleType Type { get; set; }

        public Language Language { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public Article()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
