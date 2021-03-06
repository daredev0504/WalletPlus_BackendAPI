using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IWalletRepository : IGenericRepository<Wallet>
    {

        public string GetUserId();

        bool CheckWallet(Guid walletId);

        List<Wallet> GetAllMyWallets();

        Wallet GetWalletById(Guid? id);

        List<Wallet> GetWalletsById(Guid id);

        List<Wallet> GetWalletsByUserId(string ownerId);

        Task<Wallet> GetFiatWalletById(string userId);
        Task<Wallet> GetPointWalletById(string userId);

        Wallet GetUserMainCurrencyWallet(string userId);

        bool UserHasWalletWithCurrency(FundingDto fundingDto);

        List<Wallet> GetUserWalletsByCurrencyId(string userId, int currencyId);
    }
}