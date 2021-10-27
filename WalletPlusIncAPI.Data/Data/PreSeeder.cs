using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WalletPlusIncAPI.Helpers.Rates;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.Data
{
    public static class PreSeeder
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        public static async Task Seed(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            await context.Database.EnsureCreatedAsync();

            if (!roleManager.Roles.Any())
            {
                var listOfRoles = new List<IdentityRole>
                {
                    new IdentityRole("Admin"),
                    new IdentityRole("Premium"),
                   
                };
                foreach (var role in listOfRoles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            // seed currency
            if (!context.Currencies.Any())
            {
                var request = await CurrencyRate.GetExchangeRate();
                var rates = ReflectionConverter.GetPropertyValues(request.Rates);
                var list = new List<Currency>();

                foreach (var t in rates)
                {
                    var name = ReflectionConverter.GetPropertyName(t);
                    var value = ReflectionConverter.GetPropertyValue(request.Rates, name);

                   list.Add(new Currency(){Code = name});
                }
                await context.Currencies.AddRangeAsync(list);
                await context.SaveChangesAsync();
            }

            // pre-load Admin user
            if (!userManager.Users.Any())
            {
                AppUser user = new AppUser()
                {
                    FirstName = "Peter",
                    LastName = "Tosingh",
                    UserName = "tosin@gmail.com",
                    Email = "tosin@gmail.com",
                    PhoneNumber = "09032290095",
                    Address = "Sangotedo"
                };

                var result = await userManager.CreateAsync(user, "12345Admin");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}