using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WalletPlusIncAPI.Models.Dtos.Currency;

namespace WalletPlusIncAPI.Helpers.Rates
{
    /// <summary>
    ///
    /// </summary>
    public static class CurrencyRate
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static async Task<RequestRoot> GetExchangeRate()
        {
            using HttpClient client = new HttpClient();

            using HttpResponseMessage response = await client.GetAsync("http://data.fixer.io/api/latest?access_key=c9d6219ffc703c54084500873bf7b73e&symbols");

            if (response.IsSuccessStatusCode)
            {
                using HttpContent content = response.Content;
                var myContent = await content.ReadAsStringAsync();

                try
                {
                    var value = JsonConvert.DeserializeObject<RequestRoot>(myContent);
                    return value;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="targetCode"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async Task<decimal?> ConvertCurrency(string sourceCode, string targetCode, decimal amount)
        {
            var rate = await GetExchangeRate();

            if (rate != null)
            {
                var baseRate = ReflectionConverter.GetPropertyValue(rate.Rates, rate.Base);
                var sourceRate = ReflectionConverter.GetPropertyValue(rate.Rates, sourceCode);
                var targetRate = ReflectionConverter.GetPropertyValue(rate.Rates, targetCode);

                decimal.TryParse(baseRate.ToString(), out decimal baseRateValue);
                decimal.TryParse(sourceRate.ToString(), out decimal sourceRateValue);
                decimal.TryParse(targetRate.ToString(), out decimal targetRateValue);

                // First Convert the source currency amount to its equivalent in base currency
                var sourceAmountToBaseValue = (baseRateValue * amount) / sourceRateValue;

                // Then Convert to the target currency
                var converted = (sourceAmountToBaseValue * targetRateValue) / baseRateValue;

                return converted;
            }

            return null;
        }
    }
}