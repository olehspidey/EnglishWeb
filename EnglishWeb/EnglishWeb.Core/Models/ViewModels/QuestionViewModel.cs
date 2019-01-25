using System.Collections.Generic;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class QuestionViewModel
    {
        public string Name { get; set; }
        public List<TestAnswerViewModel> Answers { get; set; }
    }
}
