using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Helpers.RequestFeatures;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.AuthManager;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly IWalletService _walletService;
      


        public AppUserService(IServiceProvider serviceProvider, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _authenticationManager = serviceProvider.GetRequiredService<IAuthenticationManager>();
            _httpContextAccessor = httpContextAccessor;
            
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _loggerService = serviceProvider.GetRequiredService<ILoggerService>();
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
           

        }

        public string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<ServiceResponse<AppUserReadDto>> SignUp(AppUserRegisterDto model, IUrlHelper url, string requestScheme)
        {
            ServiceResponse<AppUserReadDto> response = new ServiceResponse<AppUserReadDto>();

            var domainAppUser = _mapper.Map<AppUser>(model);
            domainAppUser.Created_at = DateTime.Now;

            var result = await _userManager.CreateAsync(domainAppUser, model.Password);

            if (result.Succeeded)
            {
               

                await _userManager.AddToRoleAsync(domainAppUser, "Free");
                var domainAppReadUser = _mapper.Map<AppUserReadDto>(domainAppUser);

                Wallet wallet = new Wallet()
                {
                    Balance = 0,
                    CurrencyId = model.MainCurrencyId,
                    IsMain = true,
                    OwnerId = domainAppReadUser.Id
                };

                await _walletService.AddWallet(wallet);
                //await _emailSender.SendEmailAsync(message);

                response.Message = "user created successfully";
                response.Success = true;
                response.Data = domainAppReadUser;
               
                return response;
            }
            else
            {
                response.Message = "A problem occured";
                response.Success = false;
                response.Errors = result.Errors;
                response.Data = null;
                return response;
            }

        }

      

        public async Task<ServiceResponse<string>> UpdateUser(AppUser user, AppUserUpdateDto model)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            
            if (user == null)
            {
                response.Message = "Sorry! You cannot perform this operation";
                return response;
            }
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.UserName = model.UserName ?? user.UserName;
            user.Email = model.Email ?? user.Email;
            user.Address = model.Address ?? user.Address;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                response.Message = "Update Successful";
                response.Success = true;
            }
            else
                response.Message = "Could not update user";

            return response;
        }
        public async Task<PagedList<AppUserReadDto>> GetUsers(AppUserParameters parameters)
        {
            var users = await _userManager.Users.ToListAsync();
            var appReadDtoList = _mapper.Map<ICollection<AppUserReadDto>>(users);

            return await Task.FromResult(PagedList<AppUserReadDto>.ToPagedList(appReadDtoList,
                parameters.PageNumber,
                parameters.PageSize));
        }

        
        public async Task<ServiceResponse<AppUser>> GetUser(string id)
        {
            ServiceResponse<AppUser> response = new ServiceResponse<AppUser>();

            var user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                response.Message = "Sorry! cannot find this user";
                return response;
            }

            response.Message = "User gotten";
            response.Success = true;
            response.Data = user;

            return response;
        }

        public async Task<ServiceResponse<AppUserReadDto>> GetMyDetails()
        {
            ServiceResponse<AppUserReadDto> response = new ServiceResponse<AppUserReadDto>();

            var user = GetUserId();
            var usertoReturn = await GetUser(user);
            if (usertoReturn.Data == null)
            {
                response.Message = "Sorry! cannot find this user";
                return response;
            }

            var userRead = _mapper.Map<AppUserReadDto>(usertoReturn.Data);
            response.Message = "User gotten";
            response.Success = true;
            response.Data = userRead;

            return response;
        }

        public async Task<ServiceResponse<AppUserReadDto>> FindAppUserByEmail(string email)
        {
            ServiceResponse<AppUserReadDto> response = new ServiceResponse<AppUserReadDto>();

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var userReadDto = _mapper.Map<AppUserReadDto>(user);
                response.Message = "user returned";
                response.Success = true;
                response.Data = userReadDto;
                return response;
            }

            response.Success = false;
            response.Message = "User not found";
            return response;

        }
        public async Task<ServiceResponse<string>> DeleteUser(string id)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                response.Message = "Sorry! You cannot find user";
                return response;
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                response.Message = "User deleted";
                response.Success = true;
                return response;
            }

            else
            {
                response.Message = "A problem occured";
                response.Success = false;
                return response;
            }
        }
        public Task<IList<string>> GetUserRoles(AppUser user) => _userManager.GetRolesAsync(user);

        public async Task<ServiceResponse<string>> ChangeUserRole(string userId, ChangeUserAccountTypeDto model)
        {
            var response = new ServiceResponse<string>();
            var user = await _userManager.FindByIdAsync(userId);

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                response.Message = ($"user with {userId} could not be found");
                return response;
            }

            if (!(await _userManager.IsInRoleAsync(user, model.NewType)))
            {
                await _userManager.RemoveFromRoleAsync(user, currentRoles.Where(x => x == model.CurrentType).ToString());
                await _userManager.AddToRoleAsync(user, model.NewType);
                response.Message = "user role has been changed";
                response.Success = true;
            }
            else
                response.Message = "user already has this role";

            return response;
        }


        public void AddUserToRole(AppUser user, string role)
        {
            _userManager.AddToRoleAsync(user, role);
        }

      
        public async Task<ServiceResponse<string>> ChangePassword(ChangePasswordDto model)
        {
           var response = new ServiceResponse<string>();
            if (model == null)
            {
                response.Message = "All fields are required";
                return response;
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                response.Message = "Confirm password doesn't match new password";
                return response;
            }

            var loggedInUser = GetUserId();
            var appUser = await FindAppUserByEmail(loggedInUser);
            var domainAppUser = _mapper.Map<AppUser>(appUser.Data);

            var result = await _userManager.ChangePasswordAsync(domainAppUser, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                response.Message = "user password changed successfully";
                response.Success = true;
            }
            else
                response.Message = "Invalid credentials";

            return response;
        }

     
       

        public async Task<string> GenerateEmailConfirmationLink(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public Task ForgotPassword(ForgotPasswordDto model, string origin)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(ResetPasswordDto model)
        {
            throw new NotImplementedException();
        }
    }
}
