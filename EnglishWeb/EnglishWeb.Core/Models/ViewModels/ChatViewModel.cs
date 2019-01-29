using System;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class ChatViewModel
    {
        public Guid Id { get; set; }
        public Guid UserDestinationId { get; set; }

        public string Name { get; set; }
    }
}
