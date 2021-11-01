using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// Transaction Controller
    /// </summary>
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionService"></param>
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Allows only logged-in Premium account holders to get all transactions made on every of their wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpGet]
        public async Task<IActionResult> GetMyTransaction()
        {
            var result = await _transactionService.GetMyTransactionsAsync();
            if (result.Success)
            {
                    return Ok(result);
            }
            return BadRequest(result);
        
        }

        /// <summary>
        /// Get transactions from a specific wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium,Admin")]
        [Route("{walletId}/getTransactionByWallet")]
        [HttpGet]
        public async Task<IActionResult> GetWalletTransaction(Guid walletId)
        {
            var result = await _transactionService.GetWalletTransactionsAsync(walletId);

            if (result.Success)
            {
                    return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// get credit transactions
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [Route("{walletId}/getCreditTransactionByWallet")]
        [HttpGet]
        public async Task<IActionResult> GetWalletCreditTransaction(Guid walletId)
        {
            var transactions = await _transactionService.GetWalletTransactionsByCreditAsync(walletId);

            return Ok(ResponseMessage.Message("List of all credit transactions in this wallet", null, transactions));
        }

        /// <summary>
        /// get debit transactions
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [Route("{walletId}/getDebitTransactionByWallet")]
        [HttpGet]
        public async Task<IActionResult> GetWalletDebitTransaction(Guid walletId)
        {
            var transactions = await _transactionService.GetWalletTransactionsByDebitAsync(walletId);
          
            return Ok(ResponseMessage.Message("List of all debit transactions in this wallet", null, transactions));
        }
    }
}