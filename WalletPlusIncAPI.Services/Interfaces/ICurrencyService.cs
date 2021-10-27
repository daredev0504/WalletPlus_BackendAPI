using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<string> GetCurrencyCode(int? currencyId);

        Task<ServiceResponse<bool>> CurrencyExist(int currencyId);

        Task<ServiceResponse<Currency>> GetCurrencyById(int? id);

        Task<ServiceResponse<List<Currency>>> GetAllCurrencies();

        Task<ServiceResponse<bool>> AddCurrency(Currency currency);

        Task<ServiceResponse<bool>> DeleteCurrency(int id);

        Task<ServiceResponse<bool>> UpdateCurrency(Currency currency);
    }
}
