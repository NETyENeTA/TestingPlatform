
using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.Student, opt => opt.Ignore());
    }
}