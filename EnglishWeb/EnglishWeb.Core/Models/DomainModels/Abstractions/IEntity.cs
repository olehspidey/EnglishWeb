using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishWeb.Core.Models.DomainModels.Abstractions
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }
    }
}
