using HotPot23API.Models.DTOs;

namespace HotPot23API.Interfaces
{
    public interface IAdminService
    {
        Task<RestaurantResponseDTO> AddRestaurantAsync(AdminRestaurantRegisterDTO dto);
        Task<RestaurantResponseDTO> UpdateRestaurantAsync(int restaurantId, AdminRestaurantRegisterDTO dto);
        Task<bool> DeleteRestaurantAsync(int restaurantId);

        Task<PaginatedUserResponseDTO> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<PaginatedRestaurantResponseDTO> GetAllRestaurantsAsync(int pageNumber, int pageSize);

    }
}
