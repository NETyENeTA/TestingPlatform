using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Groups, opt => opt.Ignore())
            .ForMember(dest => dest.Tests, opt => opt.Ignore());
    }
}