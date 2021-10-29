using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Helpers.RequestFeatures;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Dtos.Currency;
using WalletPlusIncAPI.Models.Dtos.Funding;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IFundingService _fundingService;
        private readonly IWalletService _walletService;
        private readonly IAppUserService _appUserService;


        public AdminController(IFundingService fundingService, IWalletService walletService, IAppUserService appUserService)
        {
            _fundingService = fundingService;
            _walletService = walletService;
            _appUserService = appUserService;
        }

        /// <summary>
        /// Allows admins to get all User details
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] AppUserParameters appUserParameters)
        {
            var users = await _appUserService.GetUsers(appUserParameters);
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

            return Ok(ResponseMessage.Message("List of all wallets", null, allWallets.Data.ToList()));
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

            return Ok(ResponseMessage.Message("List of all Free fundings yet to be approved", null, fundings));
        }

        /// <summary>
        /// Admin can change the main currency of a User
        /// </summary>
        /// <param name="changeMainCurrencyDto"></param>
        /// <returns>Response</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("changeUserMainCurrency")]
        public async Task<IActionResult> ChangeUserMainCurrency(ChangeMainCurrencyDto changeMainCurrencyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Invalid Model", ModelState));

            var old = _walletService.GetWalletById(changeMainCurrencyDto.OldMainCurrencyWalletId);
            var @new = _walletService.GetWalletById(changeMainCurrencyDto.NewMainCurrencyWalletId);

            if (old.Data == null || @new.Data == null)
                return BadRequest(ResponseMessage.Message("one of the ids entered is incorrect", "wallet to found", changeMainCurrencyDto));

            if (old.Data.OwnerId != @new.Data.OwnerId)
                return BadRequest(ResponseMessage.Message("Wallets user do not match", "Wallets does not belong to the same user", changeMainCurrencyDto));

            var changed = await _walletService.ChangeMainCurrency(old.Data, @new.Data);
            if (!changed.Success)
                return BadRequest(ResponseMessage.Message("Unable to change main currency", "error encountered while trying to save main currency", changeMainCurrencyDto));

            return Ok(ResponseMessage.Message("Main currency changed successfully", null, changeMainCurrencyDto));
        }

       /// <summary>
       /// Admin can Promote or demote an account type
       /// </summary>
       /// <param name="userId"></param>
       /// <param name="changeUserAccountTypeDto"></param>
       /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("change-role/{userId}")]
        public async Task<IActionResult> ChangeUserAccountType(string userId, ChangeUserAccountTypeDto changeUserAccountTypeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Invalid Model", ModelState));

            var user =await _appUserService.GetUser(changeUserAccountTypeDto.UserId);
            if (user == null)
                return BadRequest(ResponseMessage.Message("Invalid user Id", "user with the id was not found", changeUserAccountTypeDto));

           

            var roles = await _appUserService.GetUserRoles(user.Data);
            var oldRole = roles.FirstOrDefault();

            if (roles.Count < 0)
                _appUserService.AddUserToRole(user.Data, changeUserAccountTypeDto.NewType);
            else
                await _appUserService.ChangeUserRole(userId, changeUserAccountTypeDto);

            if (changeUserAccountTypeDto.NewType == "Free")
            {
                await _walletService.MergeAllWalletsToMain(user.Data);
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
        public async Task<IActionResult> ApproveFunding(ApproveFundingDto approveFundingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Invalid Model", ModelState));

            var funding = _fundingService.GetFundingById(approveFundingDto.FundingId);
            if (funding == null)
                return BadRequest(ResponseMessage.Message("Invalid funding Id", "funding with the id was not found", approveFundingDto));

            var wallet = _walletService.GetWalletById(funding.DestinationId);

            if (wallet == null)
                return BadRequest(ResponseMessage.Message("Invalid wallet Id", "wallet with the id was not found", approveFundingDto));

            var funded = await _walletService.FundPremiumWallet(funding);

            if (!funded.Success)
                return BadRequest(ResponseMessage.Message("Unable to fund account", "and error was encountered while trying to fund this account", approveFundingDto));

            await _fundingService.DeleteFunding(approveFundingDto.FundingId);

            return Ok(ResponseMessage.Message("Free account funded", null, approveFundingDto));
        }

       

    }
}