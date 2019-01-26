using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class CreateTestViewModel
    {
        [MinLength(2)]
        public string Name { get; set; }

        [MinLength(1)]
        public List<QuestionViewModel> Questions { get; set; }

        public TestType Type { get; set; }
    }
}
