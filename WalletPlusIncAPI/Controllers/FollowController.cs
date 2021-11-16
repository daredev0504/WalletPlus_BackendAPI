using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Helpers.RequestFeatures;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Dtos.Follow;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class FollowController : BaseApiController
    {

        private readonly IFollowService _followService;
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;

        public FollowController(IServiceProvider serviceProvider)
        {
            _followService = serviceProvider.GetRequiredService<IFollowService>();
            _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        /// <summary>
        /// get all Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] AppUserParameters appUserParameters)
        {
            var users = await _appUserService.GetUsersAsync(appUserParameters);
            return Ok(ResponseMessage.Message("List of all users", null, users));
        }

        /// <summary>
        /// get a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await _appUserService.GetUserAsync(userId);
            var usertoReturn = _mapper.Map<AppUserReadDto>(result.Data);

            return Ok(usertoReturn);
        }

        /// <summary>
        /// follow a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("follow")]
        public async Task<IActionResult> Follow(ToBeFollowedDto model)
        {
            var result = await _followService.FollowAsync(model.toBeFollowedId);
            if (result == false)
            {
                return BadRequest(ResponseMessage.Message("Already Followed", null));
            }

            return Ok(ResponseMessage.Message("Followed Succesfully", null, result));
        }

        /// <summary>
        /// unfollow a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("unfollow")]
        public async Task<IActionResult> UnFollow(ToBeUnFollowedDto model)
        {
            var result = await _followService.UnFollowAsync(model.toBeUnFollowedId);
            if (result == false)
            {
                return BadRequest(ResponseMessage.Message("error Unfollowing this user", null));
            }

            return Ok(ResponseMessage.Message("UnFollowed Succesfully", null));
        }

        /// <summary>
        /// get the number of users following you
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("followers")]
        public async Task<IActionResult> GetFollowers()
        {
            var result = await _followService.GetFollowersAsync();

            return Ok(ResponseMessage.Message($"you have {result} followers", null, result));
        }

        /// <summary>
        /// get the number of users you are following
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("following")]
        public async Task<IActionResult> GetFollowing()
        {
            var result = await _followService.GetFollowingAsync();

            return Ok(ResponseMessage.Message($"you are following {result} users", null, result));
        }
    }
}
