using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.Transaction;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ServiceResponse<List<TransactionReadDto>>> GetMyTransactionsAsync();

        Task<ServiceResponse<bool>> CreateTransactionAsync(TransactionType type, decimal amount, Guid walletId,
            int? currencyId);

        Task<ServiceResponse<bool>> DeleteTransactionAsync(Guid id);

        ServiceResponse<bool> CheckTransaction(Guid transactionId);

        Task<ServiceResponse<Transaction>> GetTransactionByIdAsync(Guid id);

        Task<ServiceResponse<List<TransactionReadDto>>> GetWalletTransactionsAsync(Guid walletId);
        Task<List<TransactionReadDto>> GetWalletTransactionsByCreditAsync(Guid walletId);
        Task<List<TransactionReadDto>> GetWalletTransactionsByDebitAsync(Guid walletId);

        Task<ServiceResponse<List<Transaction>>> GetAllTransactionsAsync();
    }
}
