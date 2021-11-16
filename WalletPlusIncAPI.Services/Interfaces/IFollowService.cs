using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IFollowService
    {
        Task<bool> FollowAsync(string followedId);
        Task<bool> UnFollowAsync(string followedId);

        Task<int> GetFollowersAsync();
        Task<int> GetFollowingAsync();
    }
}
