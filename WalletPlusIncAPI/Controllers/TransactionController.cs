using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
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
        /// Allows only logged-in Premium and Free account holders to get all transactions made on every of their wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [HttpGet]
        public async Task<IActionResult> GetMyTransaction()
        {
            var transactions = await _transactionService.GetMyTransactions();

            return Ok(ResponseMessage.Message("List of all my transaction", null, transactions.Data));
        }

        /// <summary>
        /// Get transactions from a specific wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Premium")]
        [Route("{walletId}/getTransactionByWallet")]
        [HttpGet]
        public async Task<IActionResult> GetWalletTransaction(Guid walletId)
        {
            var transactions = await _transactionService.GetWalletTransactions(walletId);

            return Ok(ResponseMessage.Message("List of all transaction in this wallet", null, transactions.Data));
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
            var transactions = await _transactionService.GetWalletTransactionsByCredit(walletId);

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
            var transactions = await _transactionService.GetWalletTransactionsByDebit(walletId);

            return Ok(ResponseMessage.Message("List of all debit transactions in this wallet", null, transactions));
        }
    }
}