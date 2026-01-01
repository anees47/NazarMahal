using AutoMapper;
using NazarMahal.Application.DTOs.AppointmentDto;
using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Application.RequestDto.GlassesRequestDto;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.Entities;
using NazarMahal.Core.ReadModels;

namespace NazarMahal.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GlassesCategory, GlassesCategoryDto>();
            CreateMap<CreateNewGlassesCategoryRequestDto, GlassesCategory>();
            CreateMap<GlassesSubCategory, GlassesSubCategoryDto>().ReverseMap();
            CreateMap<GlassesSubCategory, GlassesSubcategoriesListDto>();
            CreateMap<Glasses, GlassesDto>();
            CreateMap<GlassesAttachmentReadModel, GlassesAttachmentDto>();
            CreateMap<GlassesAttachmentReadModel, GlassesListDto>();
            CreateMap<GlassesReadModel, GlassesListDto>();
            CreateMap<GlassesSubcategoryReadModel, GlassesSubcategoriesListDto>();
            CreateMap<GlassesAttachmentReadModel, GlassesAttachmentDto>();

            CreateMap<GlassesReadModel, GlassesListDto>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.AttachmentReadModels));

            // ApplicationUser mappings are in Infrastructure layer
            CreateMap<UserResponseDto, UpdateUserRequestDto>();
            CreateMap<UserResponseDto, UserListResponseDto>();
            CreateMap<UpdateUserRequestDto, UserResponseDto>();



            CreateMap<CreateOrderRequestDto, Order>()
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

            CreateMap<OrderItemRequestDto, OrderItem>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderCreatedDate.ToDateTime(src.OrderCreatedTime)))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<Appointment, AppointmentDto>();
        }
    }
}
