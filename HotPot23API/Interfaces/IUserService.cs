using HotPot23API.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPot23API.Interfaces
{
    public interface IUserService
    {
      
        Task<PaginatedResponseDTO<UserRestaurantDTO>> GetAllRestaurantsAsync(int pageNumber = 1, int pageSize = 10);

        Task<PaginatedResponseDTO<UserMenuItemResponseDTO>> GetMenuByRestaurantAsync(
            int? restaurantId = null,
            string restaurantName = null,
            string categoryName = null,
            bool? isVeg = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int pageNumber = 1,
            int pageSize = 10);



        Task<PaginatedResponseDTO<UserMenuItemResponseDTO>> SearchMenuItemsAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);


    
        Task<UserMenuItemResponseDTO> GetMenuItemDetailsAsync(int menuItemId);

        Task<CartItemDTO> AddToCartAsync(int userId, AddToCartDTO addToCartDto);


        Task<CartResponseDTO> GetCartAsync(int userId);

        Task<CartItemDTO> UpdateCartItemAsync(int userId, UpdateCartItemDTO updateCartItemDto);

   
        Task<bool> RemoveCartItemAsync(int userId, int cartId);

      
        Task<OrderResponseDTO> CheckoutAsync(int userId, CheckoutDTO checkoutDto);

        Task<string> AddReviewAsync(int userId, AddReviewDTO dto);
        //Task<IEnumerable<UserRestaurantDTO>> GetRestaurantsByMenuAsync(string menuName);

        Task<List<UserAddressDTO>> GetUserAddressesAsync(int userId);
        Task<UserAddressDTO> AddUserAddressAsync(int userId, UserAddressDTO address);
        Task<IEnumerable<OrderResponseDTO>> GetUserOrdersAsync(int userId);



    }
}
