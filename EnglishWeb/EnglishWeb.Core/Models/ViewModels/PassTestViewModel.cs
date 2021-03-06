﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class PassTestViewModel
    {
        [Required]
        public Guid Id { get; set; }

        public List<Guid> AnswersId { get; set; }

        public List<string> Answers { get; set; }

        [Required]
        public TestType Type { get; set; }
    }
}
