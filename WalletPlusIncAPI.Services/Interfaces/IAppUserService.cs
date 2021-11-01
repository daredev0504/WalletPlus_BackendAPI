using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers.RequestFeatures;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IAppUserService
    {
        string GetUserId();
        Task<ServiceResponse<AppUserReadDto>> SignUpAsync(AppUserRegisterDto model);
       
        Task<ServiceResponse<string>> UpdateUserAsync(AppUser user, AppUserUpdateDto model);
        Task<PagedList<AppUserReadDto>> GetUsersAsync(AppUserParameters parameters);
        Task<ServiceResponse<AppUser>> GetUserAsync(string id);
        Task<ServiceResponse<AppUserReadDto>> GetMyDetailsAsync();
        Task<ServiceResponse<string>> DeleteUserAsync(string id);
        Task<ServiceResponse<AppUserReadDto>> FindAppUserByEmailAsync(string email);
        Task<IList<string>> GetUserRolesAsync(AppUser user);
        Task<ServiceResponse<string>> ChangeUserRoleAsync(string userId, ChangeUserAccountTypeDto model);
        void AddUserToRole(AppUser user, string role);
   
        Task<ServiceResponse<string>> ChangePasswordAsync(ChangePasswordDto model);
        Task<ServiceResponse<string>> ActivateUserAsync(string id);
        Task<ServiceResponse<string>> DeactivateUserAsync(string id);
        Task<bool> IsUserActiveAsync();
       
     
    }
}
