using System;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class PassedTest : Entity
    {
        [Range(0, int.MaxValue)]
        public int TrueAnswersCount { get; set; }

        [Range(0, int.MaxValue)]
        public int FalseAnswersCount { get; set; }

        public virtual User User { get; set; }

        public Guid? UserId { get; set; }

        public virtual Test Test { get; set; }

        public Guid? TestId { get; set; }

        public static PassedTest CreateFromTest(int trueAnswersCount, int falseAnswersCount, Test test, User user)
            => new PassedTest
            {
                Test = test,
                FalseAnswersCount = falseAnswersCount,
                TrueAnswersCount = trueAnswersCount,
                TestId = test.Id,
                User = user,
                UserId = user.Id
            };
    }
}
