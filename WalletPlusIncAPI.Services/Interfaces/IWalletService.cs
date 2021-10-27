using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IWalletService
    {
        string GetUserId();
        Task<ServiceResponse<bool>> AddWallet(Wallet wallet);
        Task<ServiceResponse<bool>> DeleteWallet(Guid id);
        Task<ServiceResponse<bool>> UpdateWallet(Wallet wallet);
        Task<ServiceResponse<bool>> UpdateWallet2(WalletUpdateDto updateWalletDto);
        Task<ServiceResponse<bool>> MergeAllWalletsToMain(AppUser user);
        ServiceResponse<bool> CheckWallet(Guid walletId);

        ServiceResponse<List<WalletReadDto>> GetAllMyWallets();

        ServiceResponse<Wallet> GetWalletById(Guid? id);

        ServiceResponse<List<Wallet>> GetWalletsById(Guid id);

        ServiceResponse<List<WalletReadDto>> GetWalletsByUserId(string ownerId);

        ServiceResponse<Wallet> GetUserMainCurrencyWallet(string userId);

        ServiceResponse<List<WalletReadDto>> GetAllWallets();
        Task<ServiceResponse<bool>> FundWallet(Wallet wallet, decimal amount);
        Task<ServiceResponse<bool>> FundWallet(Wallet main, Wallet source);
        Task<ServiceResponse<bool>> FundFreeWallet(Funding funding);

        bool CanWithdrawFromWallet(decimal balance, decimal? amount);

        Task<ServiceResponse<bool>> WithdrawFromWallet(WithdrawalDto withdrawalDto);
        Task<ServiceResponse<bool>> WithdrawFromMain(string userId, decimal amount);
        
        Task<ServiceResponse<bool>> ChangeMainCurrency(Wallet oldWallet, Wallet newWallet);

        ServiceResponse<bool> UserHasWalletWithCurrency(FundingDto fundingDto);

        ServiceResponse<List<Wallet>> GetUserWalletsByCurrencyId(string userId, int currencyId);

    }
}
