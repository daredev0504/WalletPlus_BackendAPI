using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.Transaction;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ServiceResponse<List<TransactionReadDto>>> GetMyTransactions();

        Task<ServiceResponse<bool>> CreateTransaction(TransactionType type, decimal amount, Guid walletId,
            int? currencyId);

        Task<ServiceResponse<bool>> DeleteTransaction(Guid id);

        ServiceResponse<bool> CheckTransaction(Guid transactionId);

        Task<ServiceResponse<Transaction>> GetTransactionById(Guid id);

        Task<ServiceResponse<List<TransactionReadDto>>> GetWalletTransactions(Guid walletId);
        Task<List<TransactionReadDto>> GetWalletTransactionsByCredit(Guid walletId);
        Task<List<TransactionReadDto>> GetWalletTransactionsByDebit(Guid walletId);

        Task<ServiceResponse<List<Transaction>>> GetAllTransactions();
    }
}
