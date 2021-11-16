using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.Currency;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IWalletService
    {
        string GetUserId();

        Task CreateInstantWallets(string ownerId, int currencyId);
        Task<ServiceResponse<bool>> AddWalletAsync(int currencyId);
        Task<ServiceResponse<bool>> DeleteWalletAsync(Guid id);
        Task<ServiceResponse<bool>> UpdateWalletAsync(Wallet wallet);
        Task<ServiceResponse<bool>> UpdateWallet2Async(WalletUpdateDto updateWalletDto);
        Task<ServiceResponse<bool>> MergeAllWalletsToMainAsync(AppUser user);
        ServiceResponse<bool> CheckWallet(Guid walletId);

        ServiceResponse<List<WalletReadDto>> GetAllMyWallets();
         Task<MainWalletReadDto> GetMyMainWalletDetails();
        ServiceResponse<Wallet> GetWalletById(Guid? id);
        Task<Wallet> GetFiatWalletByIdAsync(string userId);
        Task<string>GetFiatWalletBalanceAsync();
        Task<string>GetPointWalletBalanceAsync();
        Task<Wallet> GetPointWalletByIdAsync(string userId);

        ServiceResponse<List<Wallet>> GetWalletsById(Guid id);

        ServiceResponse<List<WalletReadDto>> GetWalletsByUserId(string ownerId);

        ServiceResponse<Wallet> GetUserMainCurrencyWallet(string userId);

        ServiceResponse<List<WalletReadDto>> GetAllWallets();
        Task<ServiceResponse<bool>> FundWalletAsync(Wallet wallet, decimal amount);
        Task<ServiceResponse<bool>> FundWalletAsync(Wallet main, Wallet source);
        Task<ServiceResponse<bool>> FundPremiumWalletAsync(Funding funding);
        Task<ServiceResponse<bool>> FundOthersAsync(FundOthersDto fundOthersDto);
        Task<LimitTypes> AwardPremiumWalletPointAsync(decimal point);
        bool CanWithdrawFromWallet(decimal balance, decimal? amount);

        Task<ServiceResponse<bool>> WithdrawFromWalletAsync(WithdrawalDto withdrawalDto);
        Task<ServiceResponse<bool>> WithdrawFromMainAsync(string userId, decimal amount);

        Task WithdrawFromWalletInstantAsync(decimal amount);
        Task<ServiceResponse<bool>> ChangeMainCurrencyAsync(ChangeMainCurrencyDto changeMainCurrencyDto);

        ServiceResponse<List<Wallet>> GetUserWalletsByCurrencyId(string userId, int currencyId);

    }
}
