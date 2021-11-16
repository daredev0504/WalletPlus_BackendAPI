using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Implementation
{
    public class FollowRepository : GenericRepository<Followers>, IFollowRepository
    {
        private readonly ApplicationDbContext _context;

        public FollowRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Followers>> GetFollowers(string followedId)
        {
            var followers = await _context.Followers.Where(f => f.IsFollowing == true && f.FollowedId == followedId)
                .ToListAsync();
            return followers;
        }

        public async Task<List<Followers>> GetFollowing(string followerId)
        {
            var followed = await _context.Followers.Where(f => f.IsFollowing == true && f.FollowerId == followerId)
                .ToListAsync();
            return followed.ToList();
        }

        public async Task<bool> FollowerExist(string followedId, string loggedInUser) => await _context.Followers.AnyAsync(x => x.FollowedId == followedId && x.FollowerId == loggedInUser);

        public async Task<Followers> GetFollow(string followedId, string loggedInUser) => await _context.Followers
            .FirstOrDefaultAsync(x => x.FollowedId == followedId && x.FollowerId == loggedInUser);

        public async Task<bool> DeleteFollow(Followers follow)
        {
            _context.Followers.Remove(follow);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
