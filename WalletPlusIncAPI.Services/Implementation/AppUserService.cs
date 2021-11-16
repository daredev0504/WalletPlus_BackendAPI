using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Helpers.ImageService;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly IWalletService _walletService;



        public AppUserService(IServiceProvider serviceProvider)
        {
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _authenticationManager = serviceProvider.GetRequiredService<IAuthenticationManager>();
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _imageService = serviceProvider.GetRequiredService<IImageService>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _loggerService = serviceProvider.GetRequiredService<ILoggerService>();
            _walletService = serviceProvider.GetRequiredService<IWalletService>();


        }

        public string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<ServiceResponse<AppUserReadDto>> SignUpAsync(AppUserRegisterDto model)
        {
            ServiceResponse<AppUserReadDto> response = new ServiceResponse<AppUserReadDto>();

            var checkUser = await FindAppUserByEmailAsync(model.Email);

            if (checkUser.Success)
            {
                response.Message = "User with the email already exist";
                response.Success = false;
                return response;
            }

            var domainAppUser = _mapper.Map<AppUser>(model);
            domainAppUser.Created_at = DateTime.Now;

            var result = await _userManager.CreateAsync(domainAppUser, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(domainAppUser, "Free");
                var domainAppReadUser = _mapper.Map<AppUserReadDto>(domainAppUser);

                await _walletService.CreateInstantWallets(domainAppReadUser.Id, model.MainCurrencyId);

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



        public async Task<ServiceResponse<string>> UpdateUserAsync(AppUser user, AppUserUpdateDto model)
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

        public async Task<PagedList<AppUserReadDto>> GetUsersAsync(AppUserParameters parameters)
        {
            var users = await _userManager.Users.ToListAsync();
            var appReadDtoList = _mapper.Map<ICollection<AppUserReadDto>>(users);

            return await Task.FromResult(PagedList<AppUserReadDto>.ToPagedList(appReadDtoList,
                parameters.PageNumber,
                parameters.PageSize));
        }


        public async Task<ServiceResponse<AppUser>> GetUserAsync(string id)
        {
            ServiceResponse<AppUser> response = new ServiceResponse<AppUser>();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Sorry! cannot find this user";
                return response;
            }

            response.Message = "User gotten";
            response.Success = true;
            response.Data = user;

            return response;
        }

        public async Task<ServiceResponse<AppUserReadDto>> GetMyDetailsAsync()
        {
            ServiceResponse<AppUserReadDto> response = new ServiceResponse<AppUserReadDto>();

            var user = GetUserId();
            var usertoReturn = await GetUserAsync(user);
            if (usertoReturn.Data == null)
            {
                response.Success = false;
                response.Message = "Sorry! cannot find this user";
                return response;
            }

            var userRead = _mapper.Map<AppUserReadDto>(usertoReturn.Data);
            response.Message = "User gotten";
            response.Success = true;
            response.Data = userRead;

            return response;
        }

        public async Task<ServiceResponse<AppUserReadDto>> FindAppUserByEmailAsync(string email)
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
        public async Task<ServiceResponse<string>> DeleteUserAsync(string id)
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
        public Task<IList<string>> GetUserRolesAsync(AppUser user) => _userManager.GetRolesAsync(user);

        public async Task<ServiceResponse<string>> ChangeUserRoleAsync(string userId, ChangeUserAccountTypeDto model)
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

        public async Task<ServiceResponse<bool>> CreateUserRoleAsync(CreateRoleDto model)
        {
            var response = new ServiceResponse<bool>();

            var result = await _roleManager.CreateAsync(new IdentityRole(model.role));


            if (result.Succeeded)
            {
                response.Message = "user role has been created";
                response.Success = true;
                return response;
            }
            else
                response.Success = false;
                response.Message = "role not created";

            return response;
        }


        public void AddUserToRole(AppUser user, string role)
        {
            _userManager.AddToRoleAsync(user, role);
        }


        public async Task<ServiceResponse<string>> ChangePasswordAsync(ChangePasswordDto model)
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
            var appUser = await FindAppUserByEmailAsync(loggedInUser);
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

        public async Task<ServiceResponse<string>> ActivateUserAsync(string id)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.Message = "cannot find user";
                return response;
            }
            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                response.Success = true;
                response.Message = "user activated";
            }
            else
                response.Message = "Could not activate user";

            return response;
        }

        public async Task<ServiceResponse<string>> DeactivateUserAsync(string id)
        {
            var response = new ServiceResponse<string>();
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.Message = "cannot find user";
                return response;
            }
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                response.Success = true;
                response.Message = "user deactivated";
            }
            else
                response.Message = "user could not be deactivated";

            return response;
        }

        public async Task<bool> IsUserActiveAsync()
        {
            var result = await GetMyDetailsAsync();
            return result.Data.IsActive;
        }

        public async Task<ServiceResponse<ImageAddedDto>> ChangePictureAsync(AppUser user, AddImageDto model)
        {
            ServiceResponse<ImageAddedDto> response = new ServiceResponse<ImageAddedDto>();
            var result = await _imageService.UploadImageAsync(model.Image);
            string publicId = result.PublicId;
            string url = result.Url.ToString();

            if (publicId == null || url == null)
            {
                response.Message = "An error occured while trying to upload your image";
                return response;
            }

            var userToUpdate = await _userManager.FindByEmailAsync(user.Email);

            if (userToUpdate is null)
            {
                response.Message = "Could not find user";
                return response;
            }

            userToUpdate.PublicId = publicId;
            userToUpdate.AvatarUrl = url;

            var userResult = await _userManager.UpdateAsync(userToUpdate);

            if (userResult.Succeeded)
            {
                var responseDTO = new ImageAddedDto
                {
                    PublicId = publicId,
                    Url = url
                };

                response.Message = "Image Uploaded successfully!";
                response.Success = true;
                response.Data = responseDTO;
            }
            else
                response.Message = "Could not upload image";

            return response;
        }

        public async Task<LoginResult> ExternalLoginForGoogleAsync(UserLoginInfo info, GoogleJsonWebSignature.Payload payload, AppUserLoginDto appUserLoginDto, IUrlHelper url, string requestScheme)
        {
            var appUser = new AppUser { UserName = payload.GivenName, Email = payload.Email, FirstName = payload.GivenName, LastName = payload.FamilyName, AvatarUrl = payload.Picture, Created_at = DateTime.Now };
            await _userManager.CreateAsync(appUser);
            //prepare and send an email for the email confirmation
            await _userManager.AddToRoleAsync(appUser, "Free");
            await _userManager.AddLoginAsync(appUser, info);

            var roles = await _authenticationManager.GetRolesAsync(appUserLoginDto);
            var token = await _authenticationManager.CreateTokenAsync(appUser);
            var user = await FindAppUserByEmailAsync(appUserLoginDto.Email);

            await _walletService.CreateInstantWallets(user.Data.Id, 19);

            return (new LoginResult
            {
                UserName = appUserLoginDto.Email,
                Role = roles,
                AccessToken = token.AccessToken,
                Name = user.Data.UserName
            });

        }

    }
}
