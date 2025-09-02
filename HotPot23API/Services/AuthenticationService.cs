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
        private readonly IEmailService _emailService;

        public AuthenticationService(
            IRepository<int, UserMaster> userMasterRepository,
            IRepository<string, User> userRepository,
            IRepository<int, UserAddressMaster> addressRepository,
             IRepository<int, RestaurantMaster> restaurantMasterRepository,  
            ITokenService tokenService,
            IMapper mapper, IEmailService emailService)
        {
            _userMasterRepository = userMasterRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _restaurantMasterRepository = restaurantMasterRepository; 
            _tokenService = tokenService;
            _mapper = mapper;
            _emailService = emailService;
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



        //public async Task<UserRegisterResponseDTO> Register(UserRegisterDTO userRegister)
        //{
        //    try
        //    {

        //        var newUserMaster = _mapper.Map<UserMaster>(userRegister);
        //        newUserMaster.CreatedOn = DateTime.Now;
        //        newUserMaster.IsActive = true;
        //        newUserMaster.Role = userRegister.Role;

        //        newUserMaster = await _userMasterRepository.Add(newUserMaster);

        //        var address = new UserAddressMaster
        //        {
        //            UserID = newUserMaster.UserID,
        //            AddressLine = userRegister.AddressLine,
        //            City = userRegister.City,
        //            State = userRegister.State,
        //            Pincode = userRegister.Pincode,
        //            Landmark = userRegister.Landmark,
        //            AddressType = userRegister.AddressType ?? "Home",
        //            IsDefault = userRegister.IsDefault,
        //            CreatedOn = DateTime.Now
        //        };

        //        await _addressRepository.Add(address);


        //        var user = PopulateUserObject(newUserMaster);
        //        user = await _userRepository.Add(user);


        //        return new UserRegisterResponseDTO
        //        {
        //            UserID = newUserMaster.UserID,
        //            Username = newUserMaster.Username,
        //            Message = "Registered Successfully"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw new Exception("Unable to register user");
        //    }
        //}

        //private User PopulateUserObject(UserMaster userMaster)
        //{
        //    var user = new User
        //    {
        //        Username = userMaster.Username,
        //        UserID = userMaster.UserID,
        //        Role = userMaster.Role
        //    };

        //    var hmacsha256 = new HMACSHA256();
        //    user.HashKey = hmacsha256.Key;

        //    // Generate default password
        //    var userPass = "#12" + user.Username + "@12";
        //    user.Password = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(userPass));

        //    return user;
        //}

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

                // ✅ Capture plain password using out
                string plainPassword;
                var user = PopulateUserObject(newUserMaster, out plainPassword);
                user = await _userRepository.Add(user);

                // ✅ Send email with credentials
                await _emailService.SendEmailAsync(
                    newUserMaster.Email,   // send to Email
                    "Welcome to HotPot – Your Login Credentials",
                    $@"Hello {newUserMaster.Name},<br/><br/>
                Your account has been created.<br/>
                <b>Username:</b> {newUserMaster.Username}<br/>
                <b>Password:</b> {plainPassword}<br/><br/>
                Please change your password after login."
                );

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


        private User PopulateUserObject(UserMaster userMaster, out string plainPassword)
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
            plainPassword = "#12" + user.Username + "@12"; // 👈 store plain password
            user.Password = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));

            return user;
        }
        public async Task ForgotPassword(string email)
        {
            // 1) Find the user in UserMaster by Email
            var allUsers = await _userMasterRepository.GetAll();
            var userMaster = allUsers.FirstOrDefault(u => u.Email == email);

            if (userMaster == null)
                throw new NoSuchEntityException();

            // 2) Recompute the default password (same as registration)
            var plainPassword = "#12" + userMaster.Username + "@12";

            // 3) Fetch user (hashed credentials) and reset password
            var dbUser = await _userRepository.GetById(userMaster.Username);
            if (dbUser == null || dbUser.HashKey == null)
                throw new NoSuchEntityException();

            using var hmac = new HMACSHA256(dbUser.HashKey);
            dbUser.Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));

            await _userRepository.Update(dbUser.Username, dbUser);

            // 4) Send email with same credentials
            await _emailService.SendEmailAsync(
                userMaster.Email,
                "HotPot – Password Reset",
                $@"Hello {userMaster.Name},<br/><br/>
            Your login credentials have been reset.<br/>
            <b>Username:</b> {userMaster.Username}<br/>
            <b>Password:</b> {plainPassword}<br/><br/>
            Please change your password after login."
            );
        }


    }
}
