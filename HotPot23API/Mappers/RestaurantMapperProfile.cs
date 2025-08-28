using AutoMapper;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;

namespace HotPot23API.Mappers
{
    public class RestaurantMapperProfile:Profile
    {
        public RestaurantMapperProfile()
        {
            
            CreateMap<CategoryMaster, CategoryResponseDTO>();
            CreateMap<CategoryCreateDTO, CategoryMaster>();
            CreateMap<CategoryUpdateDTO, CategoryMaster>();


            CreateMap<MenuItemCreateDTO, MenuItemMaster>();
            CreateMap<MenuItemMaster, MenuItemResponseDTO>();
            CreateMap<MenuItemUpdateDTO, MenuItemMaster>()
               .ForMember(dest => dest.MenuItemID, opt => opt.Ignore()) 
               .ForMember(dest => dest.RestaurantID, opt => opt.Ignore()) 
               .ForMember(dest => dest.CreatedOn, opt => opt.Ignore()); 

            CreateMap<DeliveryStatusTransaction, DeliveryStatusDTO>();
            CreateMap<ReviewTransaction, ReviewResponseDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
    .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.Name : string.Empty));

        }
    }
}
