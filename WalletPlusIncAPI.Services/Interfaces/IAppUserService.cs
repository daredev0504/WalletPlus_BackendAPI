using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers.ImageService;
using WalletPlusIncAPI.Helpers.RequestFeatures;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IAppUserService
    {
        string GetUserId();
        Task<ServiceResponse<AppUserReadDto>> SignUpAsync(AppUserRegisterDto model);
       Task<LoginResult> ExternalLoginForGoogleAsync(UserLoginInfo info, GoogleJsonWebSignature.Payload payload, AppUserLoginDto appUserLoginDto, IUrlHelper url, string requestScheme);
        Task<ServiceResponse<string>> UpdateUserAsync(AppUser user, AppUserUpdateDto model);
        Task<PagedList<AppUserReadDto>> GetUsersAsync(AppUserParameters parameters);
        Task<ServiceResponse<AppUser>> GetUserAsync(string id);
        Task<ServiceResponse<AppUserReadDto>> GetMyDetailsAsync();
        Task<ServiceResponse<string>> DeleteUserAsync(string id);
        Task<ServiceResponse<AppUserReadDto>> FindAppUserByEmailAsync(string email);
        Task<IList<string>> GetUserRolesAsync(AppUser user);
        Task<ServiceResponse<string>> ChangeUserRoleAsync(string userId, ChangeUserAccountTypeDto model);
        Task<ServiceResponse<bool>> CreateUserRoleAsync(CreateRoleDto model);
        void AddUserToRole(AppUser user, string role);
   
        Task<ServiceResponse<string>> ChangePasswordAsync(ChangePasswordDto model);
        Task<ServiceResponse<string>> ActivateUserAsync(string id);
        Task<ServiceResponse<string>> DeactivateUserAsync(string id);
        Task<bool> IsUserActiveAsync();
        Task<ServiceResponse<ImageAddedDto>> ChangePictureAsync(AppUser user, AddImageDto model);
       
     
    }
}
