using System;

namespace EnglishWeb.Core.Models.DomainModels.Abstractions
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
