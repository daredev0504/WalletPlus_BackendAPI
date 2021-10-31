using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Implementation
{
    
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        
        public TransactionRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<List<Transaction>> GetMyTransactions() => await _context.Transactions.Include(t => t.Wallet).Where(t => t.Wallet.OwnerId == GetUserId()).ToListAsync();

        public bool CheckTransaction(Guid transactionId) => _context.Transactions.Any(t => t.Id == transactionId);

        public async Task<List<Transaction>> GetWalletTransactions(Guid walletId) => await _context.Transactions.Where(t => t.WalletId == walletId).ToListAsync();

    }
}