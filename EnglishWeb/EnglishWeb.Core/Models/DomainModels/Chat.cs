using System;
using System.Collections.Generic;
using System.Text;
using EnglishWeb.Core.Models.DomainModels.Abstractions;

namespace EnglishWeb.Core.Models.DomainModels
{
    public class Chat : Entity
    {
        public string Name { get; set; }
        public virtual List<Message> Messages { get; set; }

        public virtual User UserOwner { get; set; }

        public Guid UserOwnerId { get; set; }

        public Guid UserDestinationId { get; set; }
    }
}
