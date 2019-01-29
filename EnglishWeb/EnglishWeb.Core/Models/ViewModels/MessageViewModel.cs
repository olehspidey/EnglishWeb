using System;

namespace EnglishWeb.Core.Models.ViewModels
{
    public class MessageViewModel
    {
        public string Text { get; set; }

        public Guid OwnerId { get; set; }

        public Guid DestinationId { get; set; }

        public Guid CurrentUserId { get; set; }

        public DateTime CreateTime { get; set; }

        public string Name { get; set; }
    }
}
