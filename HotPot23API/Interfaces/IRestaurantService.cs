using HotPot23API.Models;
using HotPot23API.Models.DTOs;

namespace HotPot23API.Interfaces
{
    public interface IRestaurantService
    {
        Task<CategoryResponseDTO?> AddCategoryAsync(int restaurantId, CategoryCreateDTO dto);

        Task<CategoryResponseDTO?> UpdateCategoryAsync(int restaurantId, int categoryId, CategoryUpdateDTO dto);

        Task<bool> DeleteCategoryAsync(int restaurantId, int categoryId);

        Task<PaginatedResponseDTO<CategoryResponseDTO>> GetCategoriesByRestaurantAsync(int restaurantId, int pageNumber = 1, int pageSize = 10);


        Task<PaginatedResponseDTO<MenuItemResponseDTO>> GetAllMenuItemsAsync(int restaurantId, int pageNumber = 1, int pageSize = 10);
        Task<MenuItemResponseDTO?> GetMenuItemByIdAsync(int restaurantId, int menuItemId);
        Task<MenuItemResponseDTO?> AddMenuItemAsync(int restaurantId, MenuItemCreateDTO dto);
        Task<MenuItemResponseDTO?> UpdateMenuItemAsync(int restaurantId, int menuItemId, MenuItemUpdateDTO dto);
        Task<bool> DeleteMenuItemAsync(int restaurantId, int menuItemId);

        Task AddDiscountAsync(int restaurantId, AddDiscountDTO discountDto);

        Task<PaginatedResponseDTO<OrderResponseDTO>> GetCurrentOrdersAsync(int restaurantId, int pageNumber = 1, int pageSize = 10);

        Task<OrderResponseDTO?> UpdateOrderStatusAsync(int restaurantId, int orderId, string newStatus, int statusUpdatedBy);




        Task<PaginatedResponseDTO<OrderResponseDTO>> GetOrderHistoryAsync(int restaurantId, int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<ReviewResponseDTO>> GetAllReviewsForRestaurantAsync(int restaurantId);




    }
}
