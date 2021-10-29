using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Filters;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// Controller
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ICurrencyService _currencyService;
        private readonly IAppUserService _appUserService;
        private readonly IFundingService _fundsService;
        
        public WalletController(IWalletService walletService, ICurrencyService currencyService, IAppUserService appUserService, IFundingService fundsService)
        {
            _walletService = walletService;
            _currencyService = currencyService;
            _appUserService = appUserService;
            _fundsService = fundsService;
        }

        /// <summary>
        /// Allows logged in Premium or Free account holders to create a wallet
        /// </summary>
        /// <param name="walletDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpPost("createWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateWallet(WalletCreateDto walletDto)
        {

            var loggedInUserId = _walletService.GetUserId();

            var userWallets = _walletService.GetWalletsByUserId(loggedInUserId);

            var user = await _appUserService.GetUser(loggedInUserId);
            var userRoles = await _appUserService.GetUserRoles(user.Data);

            if (userRoles.Contains("Free") && userWallets.Data.Count > 0)
                return BadRequest(ResponseMessage.Message("Already has a wallet",
                    "your account type is only allowed to have one wallet", walletDto));

            var wallet = new Wallet()
            {
                Balance = 0,
                OwnerId = loggedInUserId,
                CurrencyId = walletDto.CurrencyId,
                IsMain = false
            };

            var created = await _walletService.AddWallet(wallet);

            if (!created.Data)
                return BadRequest(ResponseMessage.Message("Unable to create wallet",
                    "error encountered while creating wallet", walletDto));

            return Ok(created);
        }

        /// <summary>
        /// Allows logged in Premium or Free account holders to delete their wallet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpDelete("deleteWallet/{id}")]
        public async Task<IActionResult> DeleteWallet(Guid id)
        {
            var result = await _walletService.DeleteWallet(id);

            if (!result.Success)
                return BadRequest(ResponseMessage.Message("Unable to delete wallet", "error encountered while deleting the wallet", id));

            return Ok(result);
        }

        /// <summary>
        /// Allows logged in Premium or Free account holders to update their wallet
        /// </summary>
        /// <param name="walletDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Premium")]
        [HttpPut("update")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateWallet(WalletUpdateDto walletDto)
        {
            var wallet = _walletService.GetWalletById(walletDto.WalletId);

            if (wallet == null)
                return BadRequest(ResponseMessage.Message("Unable to update wallet", "invalid wallet id", walletDto));

            var loggedInUserId = _walletService.GetUserId();

            if (wallet.Data.OwnerId != loggedInUserId)
                return BadRequest(ResponseMessage.Message("Invalid", "This wallet is not owned by you", walletDto));

            var updated = await _walletService.UpdateWallet2(walletDto);

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
        public IActionResult GetWallet(Guid id)
        {
            var result = _walletService.GetWalletById(id);

            if (result == null)
                return BadRequest(ResponseMessage.Message("Wallet not found", "invalid wallet id", id));

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
        public async Task<IActionResult> FundOthersWallet(FundOthersDto fundingDto)
        {
            var currencyExist = await _currencyService.CurrencyExist(fundingDto.CurrencyId);

            if (!currencyExist.Success)
                return NotFound(ResponseMessage.Message("Currency Not found", "currency id provided is invalid", fundingDto));

            var result = await _appUserService.GetUser(fundingDto.WalletOwnerId);

            if (result.Data == null)
                return NotFound(ResponseMessage.Message("User Not found", "user id provided is invalid", fundingDto));

            //var freeWalletFunded = await _walletRepository.FundNoobWallet(fundingDto);
            var WalletFunded = await _walletService.FundOthers(fundingDto);

            if (WalletFunded.Data == false)
                return BadRequest(ResponseMessage.Message("Unable to fund wallet", "An error was encountered while trying to fund the wallet", fundingDto));

            return Ok(ResponseMessage.Message($"funds successfully sent to {fundingDto.Username}", null, fundingDto));
        }



        /// <summary>
        /// Allows Premium users to fund a wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium, Admin")]
        [HttpPost("fundPremiumWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> FundPremiumWallet(FundPremiumDto fundingDto)
        {
            var currencyExist = await _currencyService.CurrencyExist(fundingDto.CurrencyId);

            if (!currencyExist.Success)
                return NotFound(ResponseMessage.Message("Currency Not found", "currency id provided is invalid", fundingDto));

            var result = await _appUserService.GetUser(fundingDto.WalletOwnerId);

            if (result.Data == null)
                return NotFound(ResponseMessage.Message("User Not found", "user id provided is invalid", fundingDto));

            var wallet = _walletService.GetFiatWalletById(fundingDto.WalletOwnerId);
           

            if (wallet.Data == null)
                return NotFound(ResponseMessage.Message("Wallet not found", "user does not have a wallet", fundingDto));

            //var freeWalletFunded = await _walletRepository.FundNoobWallet(fundingDto);
            var freeWalletFunded = await _fundsService.CreateFunding(fundingDto, wallet.Data.Id);

            if (!freeWalletFunded)
                return BadRequest(ResponseMessage.Message("Unable to fund wallet", "An error was encountered while trying to fund the wallet", fundingDto));

            return Ok(ResponseMessage.Message("Successfully funded, waiting approval from an Admin", null, fundingDto));
        }

        /// <summary>
        /// Allows Admins/Premium users to fund a wallet
        /// </summary>
        /// <param name="fundingDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium, Admin")]
        [HttpPost("fundWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> FundWallet(FundingDto fundingDto)
        {
            
            var currencyExist = await _currencyService.CurrencyExist(fundingDto.CurrencyId);

            if (!currencyExist.Success)
                return NotFound(ResponseMessage.Message("Currency Not found", "currency id provided is invalid", fundingDto));

            var user = await _appUserService.GetUser(fundingDto.WalletOwnerId);

            if (user.Data == null)
                return NotFound(ResponseMessage.Message("User Not found", "user id provided is invalid", fundingDto));

            //var userHasCurrency = _walletRepository.UserHasWalletWithCurrency(fundingDto);

            var wallet = _walletService.GetUserWalletsByCurrencyId(fundingDto.WalletOwnerId, fundingDto.CurrencyId);

            if (wallet.Data == null)
            {
                Wallet newWallet = new Wallet()
                {
                    Balance = fundingDto.Amount,
                    CurrencyId = fundingDto.CurrencyId,
                    IsMain = false,
                    OwnerId = fundingDto.WalletOwnerId,
                };

                var walletCreated = await _walletService.AddWallet(newWallet);

                if (!walletCreated.Success)
                    return BadRequest(ResponseMessage.Message("Unable to fund wallet", "An error was encountered while trying to fund the wallet", fundingDto));

                return Ok(ResponseMessage.Message("Wallet successfully created and funded", null, fundingDto));
            }

            var walletFunded = await _walletService.FundWallet(wallet.Data.FirstOrDefault(), fundingDto.Amount);

            if (!walletFunded.Success)
                return BadRequest(ResponseMessage.Message("Unable to fund wallet", "An error was encountered while trying to fund the wallet", fundingDto));

            return Ok(ResponseMessage.Message("Wallet successfully funded", null, fundingDto));
        }

        /// <summary>
        /// Allows Premium account holder to debit their wallets
        /// </summary>
        /// <param name="withdrawalDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpPost("withdrawFromWallet")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> WithdrawFromWallet(WithdrawalDto withdrawalDto)
        {

            var currencyExist = await _currencyService.CurrencyExist(withdrawalDto.CurrencyId);

            if (!currencyExist.Success)
                return NotFound(ResponseMessage.Message("Currency Not found", "currency id provided is invalid", withdrawalDto));

            var walletExist = _walletService.CheckWallet(withdrawalDto.WalletId);

            if (!walletExist.Success)
                return NotFound(ResponseMessage.Message("Wallet Not found", "wallet id provided is invalid", withdrawalDto));

            var loggedInUserId = _walletService.GetUserId();
            var userWallets = _walletService.GetWalletsByUserId(loggedInUserId);

            if (userWallets.Data.All(w => w.Id != withdrawalDto.WalletId))
                return BadRequest(ResponseMessage.Message("Unable to withdraw from this wallet", "This wallet is not owned by you", withdrawalDto));

            var walletDebited = await _walletService.WithdrawFromWallet(withdrawalDto);

            if (!walletDebited.Success)
                return BadRequest(ResponseMessage.Message("Unable to withdraw from wallet", "An error was encountered while trying to withdraw from the wallet", withdrawalDto));

            return Ok(ResponseMessage.Message("You have successfully debited the walled", null, withdrawalDto));
        }

        /// <summary>
        /// Allows Free or Premium account holder to get all their wallet(s)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpGet("getAllMyWallets")]
        public IActionResult GetAllMyWallets()
        {
            var myWallets = _walletService.GetAllMyWallets();

            return Ok(ResponseMessage.Message("List of all wallets you own", null, myWallets));
        }

        /// <summary>
        /// Allows only Admin to get a particular wallet by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("getWalletsByUserId/{id}")]
        public IActionResult GetWalletsByUserId(string id)
        {
            var myWallets = _walletService.GetWalletsByUserId(id);

            return Ok(ResponseMessage.Message("List of all wallets owned by the user", null, myWallets));
        }
    }
}