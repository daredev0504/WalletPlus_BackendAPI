using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Interfaces
{
   public interface ICurrencyRepository : IGenericRepository<Currency>
    {

        Task<bool> CurrencyExist(int currencyId);


    }
}