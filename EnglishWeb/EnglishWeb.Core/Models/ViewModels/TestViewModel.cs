using System;
using System.Collections.Generic;
using EnglishWeb.Core.Models.DomainModels;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class TestViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<QuestionViewModel> Questions { get; set; }

        public TestType Type { get; set; }
    }
}
