using AutoMapper;
using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HotPot23API.Services
{
    public class AdminService : IAdminService
    {
        private readonly FoodDeliveryManagementContext _context;
        private readonly IMapper _mapper;

        public AdminService(FoodDeliveryManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RestaurantResponseDTO> AddRestaurantAsync(AdminRestaurantRegisterDTO dto)
        {
            int userId;

            if (dto.UserId.HasValue)
            {
                var existingUser = await _context.UserMasters.FirstOrDefaultAsync(u => u.UserID == dto.UserId.Value);

                if (existingUser == null)
                    throw new NoSuchEntityException();

                if (existingUser.Role != "Restaurant")
                    throw new Exception("User does not have Restaurant role");

                userId = existingUser.UserID;
            }
            
            else
            {
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.ContactNumber))
                    throw new Exception("Username and ContactNumber are required for new user creation");

                var newUserMaster = new UserMaster
                {
                    Username = dto.Username!,
                    Name = dto.OwnerName ?? "",
                    Email = dto.Email ?? "",
                    Gender = dto.Gender ?? "",
                    ContactNumber = dto.ContactNumber!,
                    Role = "Restaurant",

                    IsActive = true,
                    CreatedOn = DateTime.Now
                };

                _context.UserMasters.Add(newUserMaster);
                await _context.SaveChangesAsync();

           
                var loginUser = PopulateUserObject(newUserMaster);
                _context.Users.Add(loginUser);
                await _context.SaveChangesAsync();

                userId = newUserMaster.UserID;
            }
           
            if (dto.Address != null &&
                (!string.IsNullOrWhiteSpace(dto.Address.AddressLine) ||
                 !string.IsNullOrWhiteSpace(dto.Address.City)))
            {
                var userAddress = new UserAddressMaster
                {
                    UserID = userId,
                    AddressLine = dto.Address.AddressLine ?? string.Empty,
                    City = dto.Address.City ?? string.Empty,
                    State = dto.Address.State ?? string.Empty,
                    Pincode = dto.Address.Pincode ?? string.Empty,
                    Landmark = dto.Address.Landmark ?? string.Empty,
                    AddressType = dto.Address.AddressType ?? "Work",
                    IsDefault = dto.IsDefault,
                    CreatedOn = DateTime.Now
                };
                _context.UserAddressMasters.Add(userAddress);
                await _context.SaveChangesAsync();
            }


            var restaurant = _mapper.Map<RestaurantMaster>(dto);
            restaurant.UserID = userId;  
            restaurant.IsActive = true;
            restaurant.CreatedOn = DateTime.Now;

            _context.RestaurantMasters.Add(restaurant);
            await _context.SaveChangesAsync();

     
            var responseDto = _mapper.Map<RestaurantResponseDTO>(restaurant);
            responseDto.Message = "Restaurant registered successfully";

            return responseDto;
        }

        public async Task<RestaurantResponseDTO> UpdateRestaurantAsync(int restaurantId, AdminRestaurantRegisterDTO dto)
        {
            var restaurant = await _context.RestaurantMasters.FirstOrDefaultAsync(r => r.RestaurantID == restaurantId);

            if (restaurant == null)
                throw new NoSuchEntityException();

            
            _mapper.Map(dto, restaurant);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RestaurantResponseDTO>(restaurant);
            responseDto.Message = "Restaurant updated successfully";

            return responseDto;
        }

        public async Task<bool> DeleteRestaurantAsync(int restaurantId)
        {
            var restaurant = await _context.RestaurantMasters.FirstOrDefaultAsync(r => r.RestaurantID == restaurantId);

            if (restaurant == null)
                throw new NoSuchEntityException();

            restaurant.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<PaginatedUserResponseDTO> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var query = _context.UserMasters
                .Include(u => u.Addresses)
                //.Where(u => u.Role == "User" && u.IsActive);
            .Where(u => u.IsActive);

            var totalNumberOfRecords = await query.CountAsync();

            if (totalNumberOfRecords == 0)
                throw new NoEntriessInCollectionException();

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDTO>>(users);

            return new PaginatedUserResponseDTO
            {
                Users = userDtos,
                TotalNumberOfRecords = totalNumberOfRecords,
                PageNumber = pageNumber
            };
        }
        public async Task<PaginatedRestaurantResponseDTO> GetAllRestaurantsAsync(int pageNumber, int pageSize)
        {
            var query = _context.RestaurantMasters
                .Where(r => r.IsActive);

            var totalNumberOfRecords = await query.CountAsync();

            if (totalNumberOfRecords == 0)
                throw new NoEntriessInCollectionException();

            var restaurants = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var restaurantDtos = _mapper.Map<List<RestaurantResponseDTO>>(restaurants);

            return new PaginatedRestaurantResponseDTO
            {
                Restaurants = restaurantDtos,
                TotalNumberOfRecords = totalNumberOfRecords,
                PageNumber = pageNumber
            };
        }
       
        private User PopulateUserObject(UserMaster userMaster)
        {
            var user = new User
            {
                Username = userMaster.Username,
                UserID = userMaster.UserID,
                Role = userMaster.Role
            };

            var hmacsha256 = new HMACSHA256();
            user.HashKey = hmacsha256.Key;

            var userPass = "#12" + user.Username + "@12";
            user.Password = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(userPass));

            return user;
        }
    }
}
