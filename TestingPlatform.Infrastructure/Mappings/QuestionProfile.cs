
using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.AnswerType,
                       opt => opt.MapFrom(src => Enum.Parse<AnswerType>(src.AnswerType)))
            .ForMember(dest => dest.Answers,
                       opt => opt.MapFrom(src => src.Answers))
            .ReverseMap()
            .ForMember(dest => dest.AnswerType,
                       opt => opt.MapFrom(src => src.AnswerType.ToString()))
            .ForMember(dest => dest.Answers,
                       opt => opt.Ignore()); // Answers обрабатываются отдельно

        CreateMap<Answer, AnswerDto>().ReverseMap();
    }
}