using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class CreateTestViewModel
    {
        [MinLength(2)]
        public string Name { get; set; }

        [MinLength(1)]
        public List<QuestionViewModel> QuestionsList { get; set; }
    }
}
