using AutoMapper;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;

namespace HotPot23API.Mappers
{
    public class AdminMapperProfile : Profile
    {
        public AdminMapperProfile()
        {
            CreateMap<AdminRestaurantRegisterDTO, RestaurantMaster>()
    .ForMember(dest => dest.UserID, opt => opt.Ignore())  
    .ForMember(dest => dest.IsActive, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedOn, opt => opt.Ignore());

            CreateMap<RestaurantMaster, RestaurantResponseDTO>();

        }
    }
}
