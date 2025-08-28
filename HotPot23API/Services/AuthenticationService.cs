using AutoMapper;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;
using HotPot23API.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace HotPot23API.Services
{
    public class AuthenticationService : IAuthenticate
    {
        private readonly IRepository<int, UserMaster> _userMasterRepository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, UserAddressMaster> _addressRepository;
        private readonly IRepository<int, RestaurantMaster> _restaurantMasterRepository;

        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthenticationService(
            IRepository<int, UserMaster> userMasterRepository,
            IRepository<string, User> userRepository,
            IRepository<int, UserAddressMaster> addressRepository,
             IRepository<int, RestaurantMaster> restaurantMasterRepository,  
            ITokenService tokenService,
            IMapper mapper)
        {
            _userMasterRepository = userMasterRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _restaurantMasterRepository = restaurantMasterRepository; 
            _tokenService = tokenService;
            _mapper = mapper;
        }

      
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            var dbUser = await _userRepository.GetById(loginRequest.Username);
            if (dbUser == null || dbUser.HashKey == null || dbUser.Password == null)
                throw new NoSuchEntityException();

        
            HMACSHA256 hmacsha256 = new HMACSHA256(dbUser.HashKey);
            var userPass = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(loginRequest.Password));
            for (int i = 0; i < userPass.Length; i++)
            {
                if (userPass[i] != dbUser.Password[i])
                    throw new NoSuchEntityException();
            }

      
            var restaurant = await _restaurantMasterRepository.GetAll();
            var userRestaurant = restaurant.FirstOrDefault(r => r.User.Username == loginRequest.Username);

            int? restaurantId = userRestaurant?.RestaurantID;
           

            return new LoginResponseDTO
            {
                Username = loginRequest.Username,
                Role = dbUser.Role,
                Token = await _tokenService.GenerateToken(new TokenUser
                {
                    Username = loginRequest.Username,
                    Role = dbUser.Role,
                    RestaurantID = restaurantId,
                    UserID = dbUser.UserID
                })
            };
        }



        public async Task<UserRegisterResponseDTO> Register(UserRegisterDTO userRegister)
        {
            try
            {

                var newUserMaster = _mapper.Map<UserMaster>(userRegister);
                newUserMaster.CreatedOn = DateTime.Now;
                newUserMaster.IsActive = true;
                newUserMaster.Role = userRegister.Role;

                newUserMaster = await _userMasterRepository.Add(newUserMaster);

                var address = new UserAddressMaster
                {
                    UserID = newUserMaster.UserID,
                    AddressLine = userRegister.AddressLine,
                    City = userRegister.City,
                    State = userRegister.State,
                    Pincode = userRegister.Pincode,
                    Landmark = userRegister.Landmark,
                    AddressType = userRegister.AddressType ?? "Home",
                    IsDefault = userRegister.IsDefault,
                    CreatedOn = DateTime.Now
                };

                await _addressRepository.Add(address);


                var user = PopulateUserObject(newUserMaster);
                user = await _userRepository.Add(user);


                return new UserRegisterResponseDTO
                {
                    UserID = newUserMaster.UserID,
                    Username = newUserMaster.Username,
                    Message = "Registered Successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Unable to register user");
            }
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

            // Generate default password
            var userPass = "#12" + user.Username + "@12";
            user.Password = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(userPass));

            return user;
        }

    }
}
