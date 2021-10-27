using System;
using System.Collections.Generic;
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

        Wallet GetUserMainCurrencyWallet(string userId);

        bool UserHasWalletWithCurrency(FundingDto fundingDto);

        List<Wallet> GetUserWalletsByCurrencyId(string userId, int currencyId);
    }
}