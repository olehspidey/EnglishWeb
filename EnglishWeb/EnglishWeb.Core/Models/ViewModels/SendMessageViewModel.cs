using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public string ToId { get; set; }

        [Required]
        [MinLength(1)]
        public string Text { get; set; }

        [Required]
        public Guid ChatId { get; set; }
    }
}
