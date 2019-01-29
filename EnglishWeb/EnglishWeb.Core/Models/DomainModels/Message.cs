using System;
using System.ComponentModel.DataAnnotations;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class Message : Entity
    {
        public Guid UserOwnerId { get; set; }

        public Guid UserDestinationId { get; set; }

        public virtual Chat Chat { get; set; }

        public Guid ChatId { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreateTime { get; set; }

        public string Name { get; set; }

        public Message()
        {
            CreateTime = DateTime.UtcNow;
        }
    }
}
