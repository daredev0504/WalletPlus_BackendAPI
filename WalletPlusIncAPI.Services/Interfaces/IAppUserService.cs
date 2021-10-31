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
        Task<ServiceResponse<AppUserReadDto>> SignUp(AppUserRegisterDto model);
       
        Task<ServiceResponse<string>> UpdateUser(AppUser user, AppUserUpdateDto model);
        Task<PagedList<AppUserReadDto>> GetUsers(AppUserParameters parameters);
        Task<ServiceResponse<AppUser>> GetUser(string id);
        Task<ServiceResponse<AppUserReadDto>> GetMyDetails();
        Task<ServiceResponse<string>> DeleteUser(string id);
        Task<ServiceResponse<AppUserReadDto>> FindAppUserByEmail(string email);
        Task<IList<string>> GetUserRoles(AppUser user);
        Task<ServiceResponse<string>> ChangeUserRole(string userId, ChangeUserAccountTypeDto model);
        void AddUserToRole(AppUser user, string role);
   
        Task<ServiceResponse<string>> ChangePassword(ChangePasswordDto model);
         Task<ServiceResponse<string>> ActivateUser(string id);
        Task<ServiceResponse<string>> DeactivateUser(string id);
           Task<ServiceResponse<string>> IsUserActive();
       
     
    }
}
