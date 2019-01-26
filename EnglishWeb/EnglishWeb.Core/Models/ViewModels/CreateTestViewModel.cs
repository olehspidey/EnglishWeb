using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels;
using Microsoft.AspNetCore.Http;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class CreateTestViewModel
    {
        [MinLength(2)]
        public string Name { get; set; }

        [MinLength(1)]
        public List<QuestionViewModel> Questions { get; set; }

        public TestType Type { get; set; }

        public List<IFormFile> Images { get; set; }

        public string QStringified { get; set; }
    }
}
