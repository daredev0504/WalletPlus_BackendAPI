using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateUser(AppUserLoginDto userForAuth);
        Task<JwtAuthResult> CreateToken(AppUser appUser);
        Task<IList<string>> GetRoles(AppUserLoginDto model);
        Task<bool> ConfirmUserEmail(string token, string email);
       //Task<AppUser> AuthenticateExternalLoginGooggle(GoogleJsonWebSignature.Payload payload);
       
    }
}
