using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Interfaces
{
    public interface IFollowRepository : IGenericRepository<Followers>
    {
        Task<List<Followers>> GetFollowers(string followedId);
        Task<List<Followers>> GetFollowing(string followerId);
        Task<bool> FollowerExist(string followedId, string loggedInUser);
        Task<Followers> GetFollow(string followedId, string loggedInUser);
        Task<bool> DeleteFollow(Followers follow);
    }
}
