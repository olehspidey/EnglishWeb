using System;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class PassedTestViewModel
    {
        public int TrueAnswersCount { get; set; }

        public int FalseAnswersCount { get; set; }

        public TestViewModel Test { get; set; }

        public Guid? TestId { get; set; }
    }
}
