using System;
using System.Collections.Generic;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class QuestionViewModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public List<TestAnswerViewModel> Answers { get; set; }
    }
}
