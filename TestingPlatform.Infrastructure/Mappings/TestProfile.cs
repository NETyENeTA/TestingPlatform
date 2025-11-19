
using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class TestProfile : Profile
{
    public TestProfile()
    {
        CreateMap<Test, TestDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<TestType>(src.Type)))
            .ReverseMap()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<Student, StudentDto>().ReverseMap();
        CreateMap<Group, GroupDto>().ReverseMap();
        CreateMap<Direction, DirectionDto>().ReverseMap();
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<Project, ProjectDto>().ReverseMap();

        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.AnswerType, opt => opt.MapFrom(src => Enum.Parse<AnswerType>(src.AnswerType)))
            .ReverseMap()
            .ForMember(dest => dest.AnswerType, opt => opt.MapFrom(src => src.AnswerType.ToString()));

        CreateMap<Answer, AnswerDto>().ReverseMap();
        CreateMap<Attempt, AttemptDto>().ReverseMap();
        CreateMap<UserAttemptAnswer, UserAttemptAnswerDto>().ReverseMap();
        CreateMap<UserSelectedOption, UserSelectedOptionDto>().ReverseMap();
        CreateMap<UserTextAnswer, UserTextAnswerDto>().ReverseMap();
        CreateMap<TestResult, TestResultDto>().ReverseMap();
    }
}