using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserService _appUserService;
        private readonly IWalletService _walletService;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ILoggerService _loggerService;


        public AuthController(IMapper mapper, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IAppUserService appUserService, IWalletService walletService, IAuthenticationManager authenticationManager, ILoggerService loggerService)
        {
            _mapper = mapper;
            _signInManager = signInManager;
            
            _userManager = userManager;
            _appUserService = appUserService;
            _walletService = walletService;
            _authenticationManager = authenticationManager;
            _loggerService = loggerService;
        }

      /// <summary>
      /// Registers a new user automatically creating a main wallet for the user.
      /// </summary>
      /// <param name="appUserRegisterDto"></param>
      /// <returns></returns>
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(AppUserRegisterDto appUserRegisterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Make sure the required fields are filled properly", ModelState));

            var checkUser = await _appUserService.FindAppUserByEmail(appUserRegisterDto.Email);
            if (checkUser.Success)
                return BadRequest(ResponseMessage.Message("User with the email already exist"));

            var result = await _appUserService.SignUp(appUserRegisterDto);
            if (!result.Success)
            {
                return BadRequest(ResponseMessage.Message("Unable to register User"));
            }
            return Ok(ResponseMessage.Message("Account Created", null, appUserRegisterDto));
            //return RedirectToAction(nameof(SuccessRegistration));


        }


     

        /// <summary>
        ///  User with accounts can Log in
        /// </summary>
        /// <param name="appUserLogin"></param>
        /// <returns></returns>
        [HttpPost("logIn")]
        public async Task<IActionResult> Login(AppUserLoginDto appUserLogin)
        {
            if (!await _authenticationManager.ValidateUser(appUserLogin))
            {
                _loggerService.LogWarn($"{nameof(Login)}: Authentication failed. Wrong user name or password.");
                return Unauthorized("Wrong user name or password");
            }
            var userResponse = await _appUserService.FindAppUserByEmail(appUserLogin.Email);
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
      /// confirms your email
      /// </summary>
      /// <param name="token"></param>
      /// <param name="email"></param>
      /// <returns></returns>
      [HttpGet]
      [Route("confirmEmail")]
      public async Task<IActionResult> ConfirmEmail(string token, string email)
      {
          var result = await _authenticationManager.ConfirmUserEmail(token, email);
          if (result)
          {
              return Ok($"Thanks for confirming your EMAIL, email - {email}, token - {token}, you can now continue to LOGIN");
          }

          return BadRequest("error confirming email");
      }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Success")]
        public IActionResult SuccessRegistration()
        {
            return Ok("Your account has been created, A link has been sent to your Mail,  Please check your email for verification.");
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
            var result = await _appUserService.GetUser(id);

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
            var result = await _appUserService.GetMyDetails();

            return Ok(result);

        }



       



    }
}