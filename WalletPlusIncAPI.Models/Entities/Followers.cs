using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlusIncAPI.Models.Entities
{
    public class Followers : BaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("FollowerId")]
        public AppUser Follower { get; set; }

        public string FollowerId { get; set; }


        [ForeignKey("FollowedId")]
        public AppUser Followed { get; set; }

        public string FollowedId { get; set; }

        public bool IsFollowing { get; set; }
    }
}
