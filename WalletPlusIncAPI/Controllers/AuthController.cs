using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using WalletManagementAPI.Helper.MailService;
using WalletPlusIncAPI.Filters;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Helpers.MailService;
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
        private readonly IEmailSender _emailService;


        public AuthController(IServiceProvider serviceProvider)
        {
            _loggerService = serviceProvider.GetRequiredService<ILoggerService>();
              _mapper = serviceProvider.GetRequiredService<IMapper>();
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
            _authenticationManager = serviceProvider.GetRequiredService<IAuthenticationManager>();
            _emailService = serviceProvider.GetRequiredService<IEmailSender>();
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
            if (!await _authenticationManager.ValidateUserAsync(appUserLogin))
            {
                _loggerService.LogWarn($"{nameof(Login)}: Authentication failed. Wrong user name or password.");
                return Unauthorized("Wrong user name or password");
            }
            var userResponse = await _appUserService.FindAppUserByEmailAsync(appUserLogin.Email);
            var user = _mapper.Map<AppUser>(userResponse.Data);
            var roles = await _authenticationManager.GetRolesAsync(appUserLogin);
            var token = await _authenticationManager.CreateTokenAsync(user);
           

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
        /// 
        /// </summary>
        /// <param name="externalAuth"></param>
        /// <returns></returns>
        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            var result = new LoginResult();

            var payload = await _authenticationManager.VerifyGoogleTokenAsync(externalAuth);
            if (payload == null)
                return BadRequest("Invalid External Authentication.");

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {

                    var loginDto = new AppUserLoginDto()
                    {
                        Email = payload.Email
                    };
                    result = await _appUserService.ExternalLoginForGoogleAsync(info, payload, loginDto, Url, Request.Scheme);
                    Response.Cookies.Append("jwt", result.AccessToken, new CookieOptions()
                    {
                        HttpOnly = true
                    });

                    return Ok(result);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }
            var token = await _authenticationManager.CreateTokenAsync(user);
            result.AccessToken = token.AccessToken;
            return Ok(result);

            //check for the Locked out account


        }




        /// <summary>
        /// Allows only logged-in admin to get the account details of any user
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

        /// <summary>
        /// reset your password
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid Request");

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);

                return BadRequest(new { Errors = errors });
            }

            return Ok();
        }

        /// <summary>
        /// forgotten password
        /// </summary>
        /// <param name="forgotPasswordDto"></param>
        /// <returns></returns>
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid Request");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                {"token", token },
                {"email", forgotPasswordDto.Email }
            };
            var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);
            var message = new EmailMessage(new string[] { user.Email }, "Reset password token", callback);
            await _emailService.SendMyEmailAsync(message);
            return Ok();
        }





    }
}