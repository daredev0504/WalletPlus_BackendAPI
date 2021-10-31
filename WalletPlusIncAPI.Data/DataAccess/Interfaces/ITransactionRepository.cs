using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Interfaces
{
    
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<List<Transaction>> GetMyTransactions();

        bool CheckTransaction(Guid transactionId);

        Task<List<Transaction>> GetWalletTransactions(Guid walletId);

    }
}