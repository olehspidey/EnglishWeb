﻿using System;
using EnglishWeb.Core.Models.DomainModels;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class ArticleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Text { get; set; }

        public ArticleType Type { get; set; }

        public User User { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
