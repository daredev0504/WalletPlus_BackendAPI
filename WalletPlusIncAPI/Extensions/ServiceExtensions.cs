using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WalletManagementAPI.Helper.MailService;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Implementation;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;
using WalletPlusIncAPI.Services.Implementation;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentityPassword(this IServiceCollection services) =>
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                // options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                
            });

        public static void ConfigureAddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));
        }
        

        public static void ConfigureDbContextForPostgresql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetSection("PG_ConnectionStrings:DefaultConnection").Value));
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WalletPlusIncAPI", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        new List<string>()

                    }

                });
            });

        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("SECRET");
            services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                        ValidAudience = jwtSettings.GetSection("validAudience").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });

        }



        public static void ConfigureLoggerService(this IServiceCollection services) => services.AddSingleton<ILoggerService, LoggerService>();
        public static void ConfigureEmailService(this IServiceCollection services) => services.AddTransient<IEmailSender, EmailSender>();
        public static void ConfigureAppUserService(this IServiceCollection services) => services.AddScoped<IAppUserService, AppUserService>();
        public static void ConfigureImageService(this IServiceCollection services) => services.AddScoped<IImageService, ImageService>();
         public static void ConfigureFollowService(this IServiceCollection services) => services.AddScoped<IFollowService, FollowService>();
        public static void ConfigureCurrencyService(this IServiceCollection services) => services.AddScoped<ICurrencyService, CurrencyService>();
        public static void ConfigureFundingService(this IServiceCollection services) => services.AddScoped<IFundingService, FundingService>();
        public static void ConfigureWalletService(this IServiceCollection services) => services.AddScoped<IWalletService, WalletService>();
        public static void ConfigureTransactionService(this IServiceCollection services) => services.AddScoped<ITransactionService, TransactionService>();
        public static void ConfigureCurrencyRepository(this IServiceCollection services) => services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        public static void ConfigureFundsRepository(this IServiceCollection services) => services.AddScoped<IFundRepository, FundRepository>();
        public static void ConfigureTransactionRepository(this IServiceCollection services) => services.AddScoped<ITransactionRepository, TransactionRepository>();
        public static void ConfigureWalletRepository(this IServiceCollection services) => services.AddScoped<IWalletRepository, WalletRepository>();
         public static void ConfigureFollowRepository(this IServiceCollection services) => services.AddScoped<IFollowRepository, FollowRepository>();
        public static void ConfigureAuthManager(this IServiceCollection services) => services.AddScoped<IAuthenticationManager, AuthenticationManager>();

    }
}
