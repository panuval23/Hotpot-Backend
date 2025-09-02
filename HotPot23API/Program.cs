
using HotPot23API.Contexts;
using HotPot23API.Interfaces;
using HotPot23API.Mappers;
using HotPot23API.Models;
using HotPot23API.Repositories;
using HotPot23API.Services;
using HotPot3API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace HotPot23API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options => options.AddPolicy("DefaultCORS", opts =>
            {
                opts.AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowAnyOrigin();
            }));

            builder.Logging.AddLog4Net();

            #region AutoMapper
            builder.Services.AddAutoMapper(typeof(UserMapperProfile));

            builder.Services.AddAutoMapper(typeof(AdminMapperProfile));
            builder.Services.AddAutoMapper(typeof(RestaurantMapperProfile));

            #endregion

            #region DbContext
            builder.Services.AddDbContext<FoodDeliveryManagementContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
            });
            #endregion

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            #region Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:JWT"]))
                    };

                });
            #endregion
            //#region Authentication
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = false,
            //            ValidateAudience = false,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(
            //                Encoding.UTF8.GetBytes(builder.Configuration["Tokens:JWT"])
            //            ),


            //            RoleClaimType = "role"
            //        };
            //    });
            //#endregion



            #region RepositoryInjection
            builder.Services.AddScoped<IRepository<int, UserMaster>, UserMasterRepositoryDB>();
            builder.Services.AddScoped<IRepository<string, User>, UserRepository>();
            builder.Services.AddScoped<IRepository<int, RestaurantMaster>, RestaurantMasterRepositoryDB>();
            builder.Services.AddScoped<IRepository<int, UserAddressMaster>, UserAddressRepositoryDB>();
            builder.Services.AddScoped<IRepository<int, CategoryMaster>, CategoryRepositoryDB>();
            builder.Services.AddScoped<IRepository<int, CategoryMaster>, CategoryRepositoryDB>();
            builder.Services.AddScoped<IRepository<int, MenuItemMaster>, MenuItemRepositoryDB>();
            builder.Services.AddScoped<IRepository<int, CartTransaction>, CartTransactionRepositoryDB>();
            builder.Services.AddScoped<IRepository<int, OrderTransaction>, OrderTransactionRepositoryDB>();






            #endregion

            #region ServiceInjection
            builder.Services.AddScoped<IAuthenticate, AuthenticationService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();



            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseCors("DefaultCORS");

            app.UseAuthentication();
            app.UseAuthorization();
    
          


            app.MapControllers();

            app.Run();
        }
    }
}
