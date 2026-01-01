using AutoMapper;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Infrastructure.Data;

namespace NazarMahal.Infrastructure.AutoMapper
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            // ApplicationUser mappings
            CreateMap<ApplicationUser, UserResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePictureUrl))
                .ForMember(dest => dest.IsDisabled, opt => opt.MapFrom(src => src.IsDisabled));

            CreateMap<ApplicationUser, UserListResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsDisabled, opt => opt.MapFrom(src => src.IsDisabled));
        }
    }
}

