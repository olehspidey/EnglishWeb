using System.ComponentModel.DataAnnotations;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class ResetPasViewModel
    {
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
