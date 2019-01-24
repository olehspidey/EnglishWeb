using AutoMapper;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;

namespace EnglishWeb.MapperConfig
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterUserVIewModel, User>()
                .ForAllMembers(expression => expression.AllowNull());
        }
    }
}
