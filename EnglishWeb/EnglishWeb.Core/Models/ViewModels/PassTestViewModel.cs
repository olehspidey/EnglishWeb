﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class PassTestViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public List<Guid> AnswersId { get; set; }
    }
}
