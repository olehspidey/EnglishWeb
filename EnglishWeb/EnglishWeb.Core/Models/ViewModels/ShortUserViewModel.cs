using System;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class ShortUserViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
