using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Implementation
{
   
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        
        public WalletRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            
        }

        public string GetUserId() => _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool CheckWallet(Guid walletId) => _context.Wallets.Any(w => w.Id == walletId);

        public List<Wallet> GetAllMyWallets() => _context.Wallets.Include(w => w.Currency).Where(w => w.OwnerId == GetUserId()).ToList();

        //public Wallet GetWalletById(Guid? id) => _context.Wallets.Include(w => w.Currency).FirstOrDefault(w => w.Id == id);

          public List<Wallet> GetWalletsById(Guid id) => _context.Wallets.Include(w => w.Currency).Where(w => w.Id == id).ToList();
         public Wallet GetWalletById(Guid? id) => _context.Wallets.Where(w => w.WalletType == WalletType.Fiat).Include(w => w.Currency).FirstOrDefault(w => w.Id == id);
          public Wallet GetFiatWalletById(string userId) => _context.Wallets.Where(w => w.WalletType == WalletType.Fiat).Include(w => w.Currency).FirstOrDefault(w => w.OwnerId == userId);
          public Wallet GetPointWalletById(string userId) => _context.Wallets.Where(w => w.WalletType == WalletType.Point).Include(w => w.Currency).FirstOrDefault(w => w.OwnerId == userId);

        public List<Wallet> GetWalletsByUserId(string ownerId) => _context.Wallets.Where(w => w.OwnerId == ownerId).ToList();

        public Wallet GetUserMainCurrencyWallet(string userId) => _context.Wallets.FirstOrDefault(w => w.OwnerId == userId && w.IsMain);

        public bool UserHasWalletWithCurrency(FundingDto fundingDto)
        {
            return _context.Wallets.Any(w => w.CurrencyId == fundingDto.CurrencyId && w.OwnerId == fundingDto.WalletOwnerId);
        }

        public List<Wallet> GetUserWalletsByCurrencyId(string userId, int currencyId) => _context.Wallets
            .Where(w => w.CurrencyId == currencyId && w.OwnerId == userId).ToList();




    }
}