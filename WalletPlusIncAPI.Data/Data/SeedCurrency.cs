using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletPlusIncAPI.Helpers.Rates;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.Data
{
    public class SeedCurrency : IEntityTypeConfiguration<Currency>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        public async void Configure(EntityTypeBuilder<Currency> builder)
        {
            var request = await CurrencyRate.GetExchangeRate();
            var rates = ReflectionConverter.GetPropertyValues(request.Rates);

            for (int i = 0; i <= rates.Count; i++)
            {
                var name = ReflectionConverter.GetPropertyName(rates[i]);
                var value = ReflectionConverter.GetPropertyValue(request.Rates, name);

                builder.HasData(
                    new Currency { Id = i + 1, Code = name });
            }
        }
    }
}