using System;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class TestAnswerViewModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public bool IsTrue { get; set; }

        public byte[] Image { get; set; }
    }
}
