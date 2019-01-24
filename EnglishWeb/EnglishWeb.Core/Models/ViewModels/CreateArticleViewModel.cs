using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class CreateArticleViewModel
    {
        [MinLength(2), Required]
        public string Name { get; set; }

        [MinLength(10), Required]
        public string Text { get; set; }

        public ArticleType Type { get; set; }
    }
}
