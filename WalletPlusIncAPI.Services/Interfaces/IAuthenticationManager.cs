using Google.Apis.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IAuthenticationManager
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(ExternalAuthDto externalAuth);
        Task<bool> ValidateUserAsync(AppUserLoginDto userForAuth);
        Task<JwtAuthResult> CreateTokenAsync(AppUser appUser);
        Task<IList<string>> GetRolesAsync(AppUserLoginDto model);
        Task<bool> ConfirmUserEmail(string token, string email);
       //Task<AppUser> AuthenticateExternalLoginGooggle(GoogleJsonWebSignature.Payload payload);
       
    }
}
