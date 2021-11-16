using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Helpers.Rates;
using WalletPlusIncAPI.Models.Dtos.Currency;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILoggerService _loggerService;




        public WalletService(IServiceProvider serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _transactionService = serviceProvider.GetRequiredService<ITransactionService>();
            _walletRepository = serviceProvider.GetRequiredService<IWalletRepository>();
            _currencyService = serviceProvider.GetRequiredService<ICurrencyService>();
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _loggerService = serviceProvider.GetRequiredService<ILoggerService>();

        }


        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        public async Task<ServiceResponse<bool>> AddWalletAsync(int currencyId)
        {
            var response = new ServiceResponse<bool>();
            var loggedInUserId = GetUserId();

            var userWallets = GetWalletsByUserId(loggedInUserId);

            var user = await _userManager.FindByIdAsync(loggedInUserId);
            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Free") && userWallets.Data.Count > 1)
            {
                response.Message = "Already has a wallet, your account type is only allowed to have one wallet";
                response.Success = false;
                response.Data = false;
                return response;
            }

            else
            {
                Wallet wallet = new Wallet()
                {
                    Balance = 0,
                    CurrencyId = currencyId,
                    IsMain = false,
                    WalletType = WalletType.Fiat,
                    OwnerId = loggedInUserId
                };
                Wallet wallet2 = new Wallet()
                {
                    Balance = 0,
                    CurrencyId = currencyId,
                    IsMain = false,
                    WalletType = WalletType.Point,
                    OwnerId = loggedInUserId
                };

                if (await _walletRepository.Add(wallet) && await _walletRepository.Add(wallet2))
                {
                    response.Success = true;
                    response.Message = "wallets created successfully";
                    return response;
                }

                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }

        }



        public async Task<ServiceResponse<bool>> DeleteWalletAsync(Guid id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                await _walletRepository.DeleteById(id);
                response.Success = true;
                response.Message = "wallet updated successfully";
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Unable to delete wallet, error encountered while deleting the wallet";
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateWalletAsync(Wallet wallet)
        {
            var response = new ServiceResponse<bool>();

            var result = await _walletRepository.Modify(wallet);
            if (result)
            {
                response.Success = true;
                response.Message = "wallet updated successfully";
                return response;
            }

            response.Success = false;
            response.Message = "A problem occured";
            return response;

        }
        public async Task<ServiceResponse<bool>> UpdateWallet2Async(WalletUpdateDto updateWalletDto)
        {
            var response = new ServiceResponse<bool>();
            var walletToUpdate = await _walletRepository.GetById(updateWalletDto.WalletId);

            if (walletToUpdate == null)
            {
                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }
            else
            {
                if (walletToUpdate.CurrencyId != updateWalletDto.CurrencyId)
                {
                    var targetCode = await _currencyService.GetCurrencyCode(updateWalletDto.CurrencyId);
                    var sourceCode = await _currencyService.GetCurrencyCode(walletToUpdate.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, walletToUpdate.Balance);

                    walletToUpdate.CurrencyId = updateWalletDto.CurrencyId;
                    walletToUpdate.Balance = newAmount ?? 0;
                }
                var result = await _walletRepository.Modify(walletToUpdate);
                if (result)
                {
                    response.Success = true;
                    response.Message = "wallet updated successfully";
                    return response;
                }
                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }

        }

        public ServiceResponse<Wallet> GetUserMainCurrencyWallet(string userId)
        {
            var response = new ServiceResponse<Wallet>();
            var wallet = _walletRepository.GetUserMainCurrencyWallet(userId);
            if (wallet != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;

            return response;
        }




        public ServiceResponse<bool> CheckWallet(Guid walletId)
        {
            var response = new ServiceResponse<bool>();
            var wallet = _walletRepository.CheckWallet(walletId);
            if (wallet)
            {
                response.Message = "wallet found";
                response.Success = true;
                return response;
            }
            response.Message = "wallet not found";
            response.Success = false;
            return response;
        }

        public ServiceResponse<List<WalletReadDto>> GetAllMyWallets()
        {
            var response = new ServiceResponse<List<WalletReadDto>>();
            var wallets = _walletRepository.GetAllMyWallets();
            var walletToReturn = _mapper.Map<List<WalletReadDto>>(wallets);

            if (wallets != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                response.Data = walletToReturn;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

        public ServiceResponse<Wallet> GetWalletById(Guid? id)
        {
            var response = new ServiceResponse<Wallet>();
            var wallet = _walletRepository.GetWalletById(id);
            if (wallet != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

        public async Task<Wallet> GetFiatWalletByIdAsync(string userId)
        {
            var wallet = await _walletRepository.GetFiatWalletById(userId);

            return wallet;

        }

        public async Task<Wallet> GetPointWalletByIdAsync(string userId)
        {
            var wallet = await _walletRepository.GetPointWalletById(userId);

            return wallet;
        }


        public ServiceResponse<List<Wallet>> GetWalletsById(Guid id)
        {
            var response = new ServiceResponse<List<Wallet>>();
            var wallets = _walletRepository.GetWalletsById(id);
            if (wallets != null)
            {
                response.Message = "wallets returned";
                response.Success = true;
                return response;
            }
            response.Message = "wallets not found";
            response.Success = false;
            response.Data = wallets;
            return response;
        }


        public ServiceResponse<List<WalletReadDto>> GetWalletsByUserId(string ownerId)
        {
            var response = new ServiceResponse<List<WalletReadDto>>();
            var wallets = _walletRepository.GetWalletsByUserId(ownerId);
            var walletsToReturn = _mapper.Map<List<WalletReadDto>>(wallets);
            if (wallets != null)
            {
                response.Message = "wallet returned";
                response.Data = walletsToReturn;
                response.Success = true;

                return response;
            }
            response.Message = "wallet not found";
            response.Success = false;
            return response;
        }


        public ServiceResponse<List<WalletReadDto>> GetAllWallets()
        {
            var response = new ServiceResponse<List<WalletReadDto>>();
            var wallets = _walletRepository.GetAll();
            if (wallets != null)
            {
                var walletsRead = _mapper.Map<IEnumerable<WalletReadDto>>(wallets);

                response.Message = "all wallets returned";
                response.Data = walletsRead.ToList();
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }


        public ServiceResponse<List<Wallet>> GetUserWalletsByCurrencyId(string userId, int currencyId)
        {
            var response = new ServiceResponse<List<Wallet>>();
            var wallet = _walletRepository.GetUserWalletsByCurrencyId(userId, currencyId);
            if (wallet != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

        public async Task<ServiceResponse<bool>> MergeAllWalletsToMainAsync(AppUser user)
        {
            var response = new ServiceResponse<bool>();

            var mainWallet = _walletRepository.GetUserMainCurrencyWallet(user.Id);
            var userWallets = _walletRepository.GetWalletsByUserId(user.Id);

            if (mainWallet != null && userWallets != null)
            {
                foreach (var wallet in userWallets)
                {
                    await FundWalletAsync(mainWallet, wallet);
                    await _walletRepository.DeleteById(wallet.Id);
                }

                response.Success = true;
                response.Message = "wallet merged";
                return response;
            }
            response.Success = false;
            response.Message = "A problem occured";
            return response;
        }


        public async Task<ServiceResponse<bool>> FundWalletAsync(Wallet wallet, decimal amount)
        {
            var response = new ServiceResponse<bool>();
            wallet.Balance += amount;

            var result = await _transactionService.CreateTransactionAsync(TransactionType.Credit, amount, wallet.Id,
                wallet.CurrencyId);
            if (result.Data)
            {
                await _walletRepository.Modify(wallet);
                response.Message = "transaction created";
                response.Success = true;
                return response;
            }
            response.Message = "transaction failed";
            _loggerService.LogError(response.Message);
            response.Success = false;
            return response;
        }

        public async Task<ServiceResponse<bool>> FundOthersAsync(FundOthersDto fundOthersDto)
        {
            var response = new ServiceResponse<bool>();
            var currencyExist = await _currencyService.CurrencyExist(fundOthersDto.CurrencyId);

            if (!currencyExist.Success)
            {
                response.Data = false;
                response.Success = false;
                response.Message = "Currency Not found, currency id provided is invalid";
                _loggerService.LogError(response.Message);
                return response;
            }

            var sender = await _userManager.FindByIdAsync(fundOthersDto.WalletOwnerId);

            if (sender == null)
            {
                response.Message = "user not found";
                _loggerService.LogError(response.Message);
                response.Data = false;
                return response;
            }
            else
            {
                var receiver = await _userManager.FindByNameAsync(fundOthersDto.Username);
                if (receiver != null)
                {
                    var receiverFiatWallet = await GetFiatWalletByIdAsync(receiver.Id);
                    await WithdrawFromWalletInstantAsync(fundOthersDto.Amount);
                    await FundWalletAsync(receiverFiatWallet, fundOthersDto.Amount);
                    response.Message = $"funds successfully sent to {fundOthersDto.Username}";
                    _loggerService.LogInfo(response.Message);
                    response.Data = true;
                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Message = "wallet not funded";
                    _loggerService.LogError(response.Message);
                    response.Data = false;
                    return response;
                }

            }
        }
        public async Task<ServiceResponse<bool>> FundWalletAsync(Wallet main, Wallet source)
        {
            var response = new ServiceResponse<bool>();
            if (main == null)
            {
                response.Success = false;
                response.Message = "an error occured";
                return response;

            }
            else
            {
                if (main.CurrencyId == source.CurrencyId)
                {
                    main.Balance += source.Balance;

                    await _transactionService.CreateTransactionAsync(TransactionType.Credit, source.Balance, main.Id,
                        source.CurrencyId);
                }
                else
                {
                    var targetCode = await _currencyService.GetCurrencyCode(main.CurrencyId);
                    var sourceCode = await _currencyService.GetCurrencyCode(source.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, source.Balance);

                    main.Balance += newAmount ?? 0;

                    await _transactionService.CreateTransactionAsync(TransactionType.Credit, newAmount ?? 0, main.Id,
                        source.CurrencyId);
                }

                await UpdateWalletAsync(main);

                response.Success = true;
                response.Message = "wallet funded successfully";
                _loggerService.LogInfo(response.Message);
                return response;
            }

        }

        // this method funds funds a wallet
        public async Task<ServiceResponse<bool>> FundPremiumWalletAsync(Funding funding)
        {
            var response = new ServiceResponse<bool>();
            var wallet = _walletRepository.GetWalletById(funding.DestinationId);


            if (wallet == null)
            {
                response.Success = false;
                response.Message = "wallet not found";
                return response;
            }
            else
            {
                var user = await _userManager.FindByIdAsync(wallet.OwnerId);

                if (funding.CurrencyId == wallet.CurrencyId)
                {
                    wallet.Balance += funding.Amount;

                    await _transactionService.CreateTransactionAsync(TransactionType.Credit, funding.Amount, wallet.Id,
                        wallet.CurrencyId);



                }
                else
                {
                    var targetCode = await _currencyService.GetCurrencyCode(wallet.CurrencyId);
                    var sourceCode = await _currencyService.GetCurrencyCode(funding.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, funding.Amount);

                    wallet.Balance += newAmount ?? 0;

                    await _transactionService.CreateTransactionAsync(TransactionType.Credit, newAmount ?? 0, wallet.Id, funding.CurrencyId);
                }

                await UpdateWalletAsync(wallet);
                response.Success = true;
                response.Message = "free wallet funded successfully";
                return response;
            }

        }

        //this method gives a user points based on how much was deposited
        public async Task<LimitTypes> AwardPremiumWalletPointAsync(decimal point)
        {
            var userId = GetUserId();
            var wallet = await GetPointWalletByIdAsync(userId);

            if (CanAddMorePointToWallet(wallet.Balance, point))
            {
                wallet.Balance += point;

                await UpdateWalletAsync(wallet);
                return LimitTypes.Normal;
            }
            if (wallet.Balance < LimitCalc.LimitForPoint && point > LimitCalc.LimitForPoint)
            {
                var rem = LimitCalc.LimitForPoint - wallet.Balance;
                wallet.Balance += rem;
                await UpdateWalletAsync(wallet);
                return LimitTypes.NotReached;
            }

            return LimitTypes.Reached;
        }

        public bool CanWithdrawFromWallet(decimal balance, decimal? amount) => (balance - amount) >= 0;
        private bool CanAddMorePointToWallet(decimal balance, decimal points) => (balance + points) <= LimitCalc.LimitForPoint;


        public async Task WithdrawFromWalletInstantAsync(decimal amount)
        {
            var wallet = await GetFiatWalletByIdAsync(GetUserId());

            wallet.Balance -= amount;
            await _transactionService.CreateTransactionAsync(TransactionType.Debit, amount, wallet.Id,
                wallet.CurrencyId);

            await UpdateWalletAsync(wallet);
        }

        public async Task<ServiceResponse<bool>> WithdrawFromWalletAsync(WithdrawalDto withdrawalDto)
        {
            var response = new ServiceResponse<bool>();
            var currencyExist = await _currencyService.CurrencyExist(withdrawalDto.CurrencyId);

            if (!currencyExist.Success)
            {
                response.Data = false;
                response.Success = false;
                response.Message = "Currency Not found, currency id provided is invalid";
                return response;
            }

            var walletExist = CheckWallet(withdrawalDto.WalletId);

            if (!walletExist.Success)
            {
                response.Success = false;
                response.Message = "Wallet Not found, wallet id provided is invalid";
                return response;
            }

            var loggedInUserId = GetUserId();
            var userWallets = GetWalletsByUserId(loggedInUserId);

            if (userWallets.Data.All(w => w.Id != withdrawalDto.WalletId))
            {
                response.Success = false;
                response.Message = "Unable to withdraw from this wallet,This wallet is not owned by you";
                return response;
            }

            var wallet = _walletRepository.GetWalletById(withdrawalDto.WalletId);

            if (wallet == null)
            {
                response.Success = false;
                response.Message = "wallet not found";
            }
            else
            {
                var user = await _userManager.FindByIdAsync(wallet.OwnerId);

                if (wallet.CurrencyId == withdrawalDto.CurrencyId)
                {
                    if (!CanWithdrawFromWallet(wallet.Balance, withdrawalDto.Amount))
                    {
                        response.Message = "you are not eligible to withdraw";
                        response.Success = false;
                        return response;
                    }
                    wallet.Balance -= withdrawalDto.Amount;

                    await _transactionService.CreateTransactionAsync(TransactionType.Debit, withdrawalDto.Amount, withdrawalDto.WalletId,
                        withdrawalDto.CurrencyId);



                    response.Message = "transaction success";
                    response.Success = true;
                    response.Data = true;
                    return response;
                }
                else
                {
                    var targetCode = await _currencyService.GetCurrencyCode(wallet.CurrencyId);
                    var sourceCode = await _currencyService.GetCurrencyCode(withdrawalDto.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, withdrawalDto.Amount);

                    if (!CanWithdrawFromWallet(wallet.Balance, newAmount))
                    {
                        response.Message = "you are not eligible to withdraw";
                        response.Success = false;
                        return response;
                    }

                    wallet.Balance -= newAmount ?? 0;

                    await _transactionService.CreateTransactionAsync(TransactionType.Debit, newAmount ?? 0, withdrawalDto.WalletId, withdrawalDto.CurrencyId);
                }

                await UpdateWalletAsync(wallet);

            }
            response.Message = "transaction success";
            response.Success = true;
            return response;

        }

        public async Task<ServiceResponse<bool>> WithdrawFromMainAsync(string userId, decimal amount)
        {
            var response = new ServiceResponse<bool>();
            var mainWallet = GetUserMainCurrencyWallet(userId);

            if (!CanWithdrawFromWallet(mainWallet.Data.Balance, amount))
            {
                response.Message = "you are not eligible to withdraw";
                response.Success = false;
                return response;
            }

            mainWallet.Data.Balance -= amount;

            await UpdateWalletAsync(mainWallet.Data);
            response.Message = "withdraw success";
            response.Success = true;
            return response;
        }


        public async Task<ServiceResponse<bool>> ChangeMainCurrencyAsync(ChangeMainCurrencyDto changeMainCurrencyDto)
        {
            var response = new ServiceResponse<bool>();
            var old = GetWalletById(changeMainCurrencyDto.OldMainCurrencyWalletId);
            var @new = GetWalletById(changeMainCurrencyDto.NewMainCurrencyWalletId);

            if (old.Data == null || @new.Data == null)
            {
                response.Success = false;
                response.Message = "one of the ids entered is incorrect, wallet not found";
                return response;
            }


            if (old.Data.OwnerId != @new.Data.OwnerId)
            {
                response.Success = false;
                response.Message = "Wallets user do not match, Wallets does not belong to the same user";
                return response;
            }

            old.Data.IsMain = false;
            @new.Data.IsMain = true;

            await UpdateWalletAsync(old.Data);
            await UpdateWalletAsync(@new.Data);

            response.Success = true;
            response.Message = "Main wallet changed";

            return response;
        }

        public async Task<string> GetFiatWalletBalanceAsync()
        {
            var wallet = await GetFiatWalletByIdAsync(GetUserId());
            return wallet.Balance.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<string> GetPointWalletBalanceAsync()
        {
            var wallet = await GetPointWalletByIdAsync(GetUserId());
            return wallet.Balance.ToString(CultureInfo.InvariantCulture);
        }

        public async Task CreateInstantWallets(string ownerId, int currencyId)
        {
            Wallet wallet = new Wallet()
            {
                Balance = 0,
                CurrencyId = currencyId,
                IsMain = true,
                WalletType = WalletType.Fiat,
                OwnerId = ownerId
            };
            Wallet wallet2 = new Wallet()
            {
                Balance = 0,
                CurrencyId = currencyId,
                IsMain = true,
                WalletType = WalletType.Point,
                OwnerId = ownerId
            };

            await _walletRepository.Add(wallet);
            await _walletRepository.Add(wallet2);
        }

        public async Task<MainWalletReadDto> GetMyMainWalletDetails()
        {
            var point = await GetPointWalletBalanceAsync();
            var fiat = await GetFiatWalletByIdAsync(GetUserId());
            if (fiat != null)
            {
                var model = new MainWalletReadDto
                {
                    PointBalance = point,
                    FiatBalance = fiat.Balance.ToString(),
                    OwnerId = fiat.OwnerId,
                    CurrencyId = fiat.CurrencyId,
                    IsMain = fiat.IsMain,
                    CurrencyCode = fiat.Currency.Code
                    
                };

                return model;
            }
            return null;
        }
    }
}
