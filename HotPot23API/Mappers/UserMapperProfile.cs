using AutoMapper;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;

namespace HotPot23API.Mappers
{
    public class UserMapperProfile:Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserRegisterDTO, UserMaster>();
            // CreateMap<UserMaster, UserDTO>()
            //.ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses)).ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role)) ;
            //        CreateMap<UserMaster, UserDTO>()
            //.ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            //.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            //.ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurants.FirstOrDefault()));

            //        CreateMap<RestaurantMaster, UserRestaurantDTO>();
            CreateMap<UserMaster, UserDTO>()
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurants.FirstOrDefault()));

            CreateMap<RestaurantMaster, UserRestaurantDTO>()
               .ForMember(dest => dest.RestaurantID, opt => opt.MapFrom(src => src.RestaurantID))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RestaurantName))
               .ReverseMap();



            CreateMap<UserAddressMaster, UserAddressDTO>();

            CreateMap<UserAddressMaster, UserAddressDTO>();

            CreateMap<RestaurantMaster, UserRestaurantDTO>()
           .ForMember(dest => dest.RestaurantID, opt => opt.MapFrom(src => src.RestaurantID))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RestaurantName))
           .ReverseMap();

            //CreateMap<MenuItemMaster, UserMenuItemResponseDTO>()
            //    .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
            //    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            //    .ReverseMap();
            CreateMap<MenuItemMaster, UserMenuItemResponseDTO>()
    .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
    .AfterMap((src, dest) =>
    {
        var now = DateTime.UtcNow;
        var activeDiscount = src.Discounts?
            .Where(d => (!d.ValidFrom.HasValue || d.ValidFrom.Value <= now)
                     && (!d.ValidTo.HasValue || d.ValidTo.Value >= now))
            .OrderByDescending(d => d.DiscountPercent ?? 0)
            .FirstOrDefault();

        if (activeDiscount != null)
        {
            dest.DiscountPercent = activeDiscount.DiscountPercent;
            dest.FinalPrice = activeDiscount.DiscountPercent.HasValue
                ? src.Price - (src.Price * activeDiscount.DiscountPercent.Value / 100)
                : src.Price;
            dest.DiscountValidFrom = activeDiscount.ValidFrom;
            dest.DiscountValidTo = activeDiscount.ValidTo;
        }
        else
        {
            dest.DiscountPercent = null;
            dest.FinalPrice = src.Price;
            dest.DiscountValidFrom = null;
            dest.DiscountValidTo = null;
        }
    });
            CreateMap<ReviewTransaction, ReviewResponseDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
    .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.Name : string.Empty));



        }
    }
}
