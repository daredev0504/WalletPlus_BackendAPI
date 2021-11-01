using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Filters;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    public class AuthController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserService _appUserService;
        private readonly IWalletService _walletService;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ILoggerService _loggerService;


        public AuthController(IServiceProvider serviceProvider)
        {
            _loggerService = serviceProvider.GetRequiredService<ILoggerService>();
              _mapper = serviceProvider.GetRequiredService<IMapper>();
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
            _authenticationManager = serviceProvider.GetRequiredService<IAuthenticationManager>();
        }

      /// <summary>
      /// Registers a new user automatically creating a main wallet for the user.
      /// </summary>
      /// <param name="appUserRegisterDto"></param>
      /// <returns></returns>
        [HttpPost("signUp")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> SignUp(AppUserRegisterDto appUserRegisterDto)
        {
            var result = await _appUserService.SignUpAsync(appUserRegisterDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }


        /// <summary>
        ///  User with accounts can Log in
        /// </summary>
        /// <param name="appUserLogin"></param>
        /// <returns></returns>
        [HttpPost("logIn")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AppUserLoginDto appUserLogin)
        {
            if (!await _authenticationManager.ValidateUser(appUserLogin))
            {
                _loggerService.LogWarn($"{nameof(Login)}: Authentication failed. Wrong user name or password.");
                return Unauthorized("Wrong user name or password");
            }
            var userResponse = await _appUserService.FindAppUserByEmailAsync(appUserLogin.Email);
            var user = _mapper.Map<AppUser>(userResponse.Data);
            var roles = await _authenticationManager.GetRoles(appUserLogin);
            var token = await _authenticationManager.CreateToken(user);
           

            Response.Cookies.Append("jwt", token.AccessToken, new CookieOptions()
            {
                HttpOnly = true
            });

            return Ok(new LoginResult
            {
                UserName = appUserLogin.Email,
                Role = roles,
                AccessToken = token.AccessToken,
                Name = user.UserName
            });
            
        }
      

        /// <summary>
        /// Allows only logged-in admin to account details of any user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getUserDetail/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _appUserService.GetUserAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Allows any logged in user to get his/her account details
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getMyDetails")]
        public  async Task<IActionResult>  GetMyDetails()
        {
            var result = await _appUserService.GetMyDetailsAsync();
            if (result.Success)
            {
               return Ok(result);
            }
            return BadRequest(result);

        }



       



    }
}