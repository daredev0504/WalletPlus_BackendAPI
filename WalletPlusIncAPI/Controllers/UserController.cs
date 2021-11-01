using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
  
    /// <summary>
    /// User Controller
    /// </summary>
    public class UserController : BaseApiController
    {
        private readonly IAppUserService _appUserService;
        private readonly UserManager<AppUser> _userManager;


        /// <summary>
        /// UserController constructor
        /// </summary>
        public UserController(IServiceProvider provider)
        {
            _appUserService = provider.GetRequiredService<IAppUserService>();
            _userManager = provider.GetRequiredService<UserManager<AppUser>>();
        
        }


        /// <summary>
        /// Updates a User in the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("update-profile")]
        public async Task<IActionResult> UpdateUser([FromBody] AppUserUpdateDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _appUserService.UpdateUserAsync(user, model);
            if (result.Success) return NoContent();
            return BadRequest(result);
        }


        /// <summary>
        /// Activate a User in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("activate-user/{userId}")]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var result = await _appUserService.ActivateUserAsync(userId);
            if (result.Success) return NoContent();
            return BadRequest(result);
        }

        /// <summary>
        /// Deactivates a User in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("deactivate-user/{userId}")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var result = await _appUserService.DeactivateUserAsync(userId);
            if (result.Success) return NoContent();
            return BadRequest(result);
        }

        /// <summary>
        /// Implements a logged in user's change password functionality
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var result = await _appUserService.ChangePasswordAsync(model);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
