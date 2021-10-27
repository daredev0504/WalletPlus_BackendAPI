using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Implementation
{
   
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        private readonly ApplicationDbContext _context;

       public CurrencyRepository(ApplicationDbContext context) : base(context) => _context = context;



       public Task<bool> CurrencyExist(int currencyId) => _context.Currencies.AnyAsync(c => c.Id == currencyId);



    }
}