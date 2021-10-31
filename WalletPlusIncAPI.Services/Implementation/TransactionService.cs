using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Dtos.Transaction;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _iMapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper iMapper)
        {
            _transactionRepository = transactionRepository;
            _iMapper = iMapper;
        }


        public async Task<ServiceResponse<List<TransactionReadDto>>> GetMyTransactions()
        {
            var response = new ServiceResponse<List<TransactionReadDto>>();
            var result =  await _transactionRepository.GetMyTransactions();
          
            if (result != null)
            {
                var orderedTransactions = result.OrderByDescending(x => x.Created_at).ToList();
                var transactionRead = _iMapper.Map<List<TransactionReadDto>>(orderedTransactions);
                response.Success = true;
                response.Message = "transaction returned successfully";
                response.Data = transactionRead;
                return response;
            }
            response.Success = false;
            response.Message = "an error occured";
            return response;
        }

        public async Task<ServiceResponse<bool>> CreateTransaction(TransactionType type, decimal amount, Guid walletId,
            int? currencyId)
        {
            var response = new ServiceResponse<bool>();
            Transaction transaction = new Transaction
            {
                Type = type,
                Amount = amount,
                Date = DateTime.Now,
                WalletId = walletId,
                CurrencyId = currencyId
            };

            if (await _transactionRepository.Add(transaction))
            {
                response.Success = true;
                response.Message = "transaction created successfully";
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "transaction failed";
                return response;
            }
        }


        public async Task<ServiceResponse<bool>> DeleteTransaction(Guid id)
        {
            var response = new ServiceResponse<bool>();
            var result = await _transactionRepository.DeleteById(id);
            if (result)
            {
                response.Success = true;
                response.Message = "transaction deleted successfully";
                return response;
            }
            response.Success = false;
            response.Message = "an error occured";
            return response;
        }

        public ServiceResponse<bool> CheckTransaction(Guid transactionId)
        {
            var response = new ServiceResponse<bool>();
            var result = _transactionRepository.CheckTransaction(transactionId);
            if (result)
            {
                response.Success = true;
                response.Message = "transaction found";
                return response;
            }
            response.Success = false;
            response.Message = "an error occured";
            return response;
        }

        public async Task<ServiceResponse<Transaction>> GetTransactionById(Guid id)
        {
            var response = new ServiceResponse<Transaction>();
            var result = await _transactionRepository.GetById(id);
            if (result != null)
            {
                response.Success = true;
                response.Message = "transaction deleted successfully";
                return response;
            }
            response.Success = false;
            response.Message = "an error occured";
            return response;
        }

        public async Task<ServiceResponse<List<TransactionReadDto>>> GetWalletTransactions(Guid walletId)
        {
            var response = new ServiceResponse<List<TransactionReadDto>>();
            var result = await _transactionRepository.GetWalletTransactions(walletId);

            if (result != null)
            {
                var orderedTransactions = result.OrderByDescending(x => x.Created_at).ToList();
                var transactionRead = _iMapper.Map<List<TransactionReadDto>>(orderedTransactions);
                response.Success = true;
                response.Message = "wallet transactions returned";
                response.Data = transactionRead;
                return response;
            }
            response.Success = false;
            response.Message = "an error occured";
            return response;
        }

        public async Task<List<TransactionReadDto>> GetWalletTransactionsByCredit(Guid walletId)
        {
            var allTransactions = await GetWalletTransactions(walletId);
            var creditTransactions = allTransactions.Data.Where(x => x.Type == "Credit").ToList();
            return creditTransactions;
        }

        public async Task<List<TransactionReadDto>> GetWalletTransactionsByDebit(Guid walletId)
        {
            var allTransactions = await GetWalletTransactions(walletId);
            var debitTransactions = allTransactions.Data.Where(x => x.Type == "Debit").ToList();
            return debitTransactions;
        }

        public async Task<ServiceResponse<List<Transaction>>> GetAllTransactions()
        {
            var response = new ServiceResponse<List<Transaction>>();
            var result =await _transactionRepository.GetAll().ToListAsync();
            if (result != null)
            {
                response.Success = true;
                response.Message = "all transactions returned";
                return response;
            }
            response.Success = false;
            response.Message = "an error occured";
            return response;
        }
    }
}
