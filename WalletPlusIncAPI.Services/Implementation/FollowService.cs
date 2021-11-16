using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class FollowService : IFollowService
    {
        private readonly IAppUserService _appUserService;
        private readonly IFollowRepository _followRepository;

        public FollowService(IAppUserService appUserService, IFollowRepository followRepository)
        {
            _appUserService = appUserService;
            _followRepository = followRepository;
        }
        public async Task<bool> FollowAsync(string followedId)
        {
            var loggedInUser = _appUserService.GetUserId();

            var followedExist = await _followRepository.FollowerExist(followedId, loggedInUser);
            if (followedExist)
            {
                return false;
            }
            else
            {
                var follow = new Followers()
                {
                    IsFollowing = true,
                    FollowerId = loggedInUser,
                    FollowedId = followedId
                };
                var result = await _followRepository.Add(follow);

                return true;
            }

        }

        public async Task<bool> UnFollowAsync(string followedId)
        {
            var loggedInUser = _appUserService.GetUserId();
            var followedExist = await _followRepository.GetFollow(followedId, loggedInUser);

            if (followedExist != null)
            {

                var result = await _followRepository.DeleteFollow(followedExist);
                return true;
            }

            return false;
        }

        public async Task<int> GetFollowersAsync()
        {
            var loggedInUser = _appUserService.GetUserId();
            var followers = await _followRepository.GetFollowers(loggedInUser);
            return followers.Count;
        }

        public async Task<int> GetFollowingAsync()
        {
            var loggedInUser = _appUserService.GetUserId();
            var following = await _followRepository.GetFollowing(loggedInUser);
            return following.Count;
        }
    }
}
