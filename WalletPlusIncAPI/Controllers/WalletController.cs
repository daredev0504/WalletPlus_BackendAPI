using System;
using System.Linq;
using System.Threading.Tasks;
using Commander.API.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Filters;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// Wallet Controller
    /// </summary>
    public class WalletController : BaseApiController
    {
        private readonly IWalletService _walletService;
        private readonly ICurrencyService _currencyService;
        private readonly IAppUserService _appUserService;
        private readonly IFundingService _fundsService;



        public WalletController(IServiceProvider serviceProvider)
        {
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
            _currencyService = serviceProvider.GetRequiredService<ICurrencyService>();
            _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
            _fundsService = serviceProvider.GetRequiredService<IFundingService>();

        }


        /// <summary>
        /// Allows logged in Premium or Free account holders to create a wallet
        /// </summary>
        /// <param name="walletCreateDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium, Free")]
        [HttpPost("createWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> CreateWallet(WalletCreateDto walletCreateDto)
        {
            var result = await _walletService.AddWalletAsync(walletCreateDto.CurrencyId);

            if (!result.Data)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Allows logged in Premium account holders to delete their wallet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpDelete("deleteWallet/{id}")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> DeleteWallet(Guid id)
        {
            var result = await _walletService.DeleteWalletAsync(id);

            if (!result.Success)
            {
                return BadRequest(ResponseMessage.Message("Unable to delete wallet", "error encountered while deleting the wallet", id));

            }


            return Ok(result);
        }

        /// <summary>
        /// Allows logged in Premium account holders to update their wallet
        /// </summary>
        /// <param name="walletDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Premium")]
        [HttpPut("update")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> UpdateWallet(WalletUpdateDto walletDto)
        {
            var wallet = _walletService.GetWalletById(walletDto.WalletId);

            if (wallet == null)
                return BadRequest(ResponseMessage.Message("Unable to update wallet", "invalid wallet id", walletDto));

            var loggedInUserId = _walletService.GetUserId();

            if (wallet.Data.OwnerId != loggedInUserId)
                return BadRequest(ResponseMessage.Message("Invalid", "This wallet is not owned by you", walletDto));

            var updated = await _walletService.UpdateWallet2Async(walletDto);

            if (!updated.Success)
                return BadRequest(ResponseMessage.Message("Unable to update wallet", "error encountered while updating the wallet", walletDto));

            return Ok(updated);
        }

        /// <summary>
        /// Allows logged-in Admin account holders to get a wallet by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getWalletDetail/{id}")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public IActionResult GetWallet(Guid id)
        {
            var result = _walletService.GetWalletById(id);

            if (result == null)
            {
                return BadRequest(ResponseMessage.Message("Wallet not found", "invalid wallet id", id));
            }


            return Ok(result);
        }

        /// <summary>
        /// send funds to other registered users
        /// </summary>
        /// <param name="fundingDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpPost("fundRegisteredUserWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> FundOthersWallet(FundOthersDto fundingDto)
        {

            var result = await _walletService.FundOthersAsync(fundingDto);
            if (result.Success == false)
                return BadRequest(result);

            return Ok(result);
        }



        /// <summary>
        /// Allows Premium users to fund a wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium, Admin")]
        [HttpPost("fundPremiumWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> FundPremiumWallet(FundPremiumDto fundingDto)
        {

            var result = await _fundsService.CreateFundingAsync(fundingDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        /// <summary>
        /// Allows Premium account holder to debit their wallets
        /// </summary>
        /// <param name="withdrawalDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpPost("withdrawFromWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> WithdrawFromWallet(WithdrawalDto withdrawalDto)
        {
            var result = await _walletService.WithdrawFromWalletAsync(withdrawalDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Allows Premium account holder to get all their wallet(s)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpGet("getAllMyWallets")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public IActionResult GetAllMyWallets()
        {
            var result = _walletService.GetAllMyWallets();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

         /// <summary>
        /// Allows Premium account holder to get all their main wallets details
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium, Free")]
        [HttpGet("getMyMainWalletsDetails")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> GetMainWlletsDetails()
        {
            var result = await _walletService.GetMyMainWalletDetails();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// get your money wallet balance
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpGet("fiatBalance")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> GetFiatWalletBalance()
        {
            var result = await _walletService.GetFiatWalletBalanceAsync();

            return Ok(ResponseMessage.Message($"your fiat wallet balance is {result} ", null, result));
        }

        /// <summary>
        ///  get your point wallet balance
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpGet("pointBalance")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public async Task<IActionResult> GetPointWallet()
        {
            var result = await _walletService.GetPointWalletBalanceAsync();
            if (result != null)
            {
                return Ok(ResponseMessage.Message($"your point balance is {result} ", null, result));
            }

            return BadRequest();
        }


        /// <summary>
        /// Allows only Admin to get a particular wallet by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getWalletsByUserId/{id}")]
        [ServiceFilter(typeof(ValidateUserActiveAttribute))]
        public IActionResult GetWalletsByUserId(string id)
        {
            var myWallets = _walletService.GetWalletsByUserId(id);

            return Ok(ResponseMessage.Message("List of all wallets owned by the user", null, myWallets));
        }
    }
}