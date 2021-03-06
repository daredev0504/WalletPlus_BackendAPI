using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Filters;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Helpers.RequestFeatures;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Dtos.Currency;
using WalletPlusIncAPI.Models.Dtos.Funding;
using WalletPlusIncAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// Admin Controller
    /// </summary>
    public class AdminController : BaseApiController
    {
        private readonly IFundingService _fundingService;
        private readonly IWalletService _walletService;
        private readonly IAppUserService _appUserService;


        public AdminController(IServiceProvider serviceProvider)
        {
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
            _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
            _fundingService = serviceProvider.GetRequiredService<IFundingService>();
        }

        /// <summary>
        /// Allows admins to get all User details
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] AppUserParameters appUserParameters)
        {
            var users = await _appUserService.GetUsersAsync(appUserParameters);
            return Ok(ResponseMessage.Message("List of all users", null, users));
        }

        /// <summary>
        /// Allows only admins to get wallet infos
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        [HttpGet("getAllWallets")]
        public IActionResult GetAllWallets()
        {
            var allWallets = _walletService.GetAllWallets();
            if (allWallets.Success)
            {
                return Ok(ResponseMessage.Message("List of all wallets", null, allWallets.Data.ToList()));
            }
            return BadRequest(allWallets);
        }

        /// <summary>
        /// Allows an admin to get all free funds yet to be approved
        /// </summary>
        /// <returns>Admin Route</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getUnApprovedFundings")]
        public IActionResult GetUnApprovedFundings()
        {
            var fundings = _fundingService.GetUnApprovedFundings();
            if (fundings != null)
            {
                return Ok(ResponseMessage.Message("List of all Free fundings yet to be approved", null, fundings));
            }

            return BadRequest(ResponseMessage.Message("Error", "could not fetch the requested resource", null));
        }

        /// <summary>
        /// Admin can change the main currency of a User
        /// </summary>
        /// <param name="changeMainCurrencyDto"></param>
        /// <returns>Response</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("changeUserMainCurrency")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ChangeUserMainCurrency(ChangeMainCurrencyDto changeMainCurrencyDto)
        {
            var result = await _walletService.ChangeMainCurrencyAsync(changeMainCurrencyDto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        /// <summary>
        /// create a new user role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("createNewUserRole")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateNewUserRole(CreateRoleDto model)
        {
            var result = await _appUserService.CreateUserRoleAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Admin can Promote or demote an account type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeUserAccountTypeDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("change-role/{userId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ChangeUserAccountType(string userId, ChangeUserAccountTypeDto changeUserAccountTypeDto)
        {
            var user = await _appUserService.GetUserAsync(changeUserAccountTypeDto.UserId);
            if (user == null)
                return BadRequest(ResponseMessage.Message("Invalid user Id", "user with the id was not found", changeUserAccountTypeDto));

            var roles = await _appUserService.GetUserRolesAsync(user.Data);
            var oldRole = roles.FirstOrDefault();

            if (roles.Count < 0)
                _appUserService.AddUserToRole(user.Data, changeUserAccountTypeDto.NewType);
            else
                await _appUserService.ChangeUserRoleAsync(userId, changeUserAccountTypeDto);

            if (changeUserAccountTypeDto.NewType == "Free")
            {
                await _walletService.MergeAllWalletsToMainAsync(user.Data);
            }

            return Ok(ResponseMessage.Message("Account type changed successfully", null, changeUserAccountTypeDto));

        }

        /// <summary>
        /// Admin can approve the funding of a Free account holder
        /// </summary>
        /// <param name="approveFundingDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("approveFunding")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ApproveFunding(ApproveFundingDto approveFundingDto)
        {
            var funding = _fundingService.GetFundingById(approveFundingDto.FundingId);
            if (funding == null)
                return BadRequest(ResponseMessage.Message("Invalid funding Id", "funding with the id was not found", approveFundingDto));

            var wallet = _walletService.GetWalletById(funding.DestinationId);

            if (wallet == null)
                return BadRequest(ResponseMessage.Message("Invalid wallet Id", "wallet with the id was not found", approveFundingDto));

            var funded = await _walletService.FundPremiumWalletAsync(funding);

            if (!funded.Success)
                return BadRequest(ResponseMessage.Message("Unable to fund account", "and error was encountered while trying to fund this account", approveFundingDto));

            await _fundingService.DeleteFundingAsync(approveFundingDto.FundingId);

            return Ok(ResponseMessage.Message("Premium account funded", null, approveFundingDto));
        }



    }
}