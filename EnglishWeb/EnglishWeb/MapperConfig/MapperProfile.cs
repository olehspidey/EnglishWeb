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
            CreateMap<CreateArticleViewModel, Article>()
                .ForAllMembers(expression => expression.AllowNull());
            CreateMap<QuestionViewModel, Question>()
                .ForAllOtherMembers(expression => expression.AllowNull());
            CreateMap<TestAnswerViewModel, TestAnswer>()
                .ForAllOtherMembers(expression => expression.AllowNull());
            CreateMap<CreateTestViewModel, Test>()
                .ForAllOtherMembers(expression => expression.AllowNull());
            CreateMap<Test, TestViewModel>()
                .ForAllMembers(expression => expression.AllowNull());
        }
    }
}
