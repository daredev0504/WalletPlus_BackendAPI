using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WalletPlusIncAPI.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
          
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime Created_at { get; set; }


        //Navigational Properties

        public ICollection<Wallet> Wallets { get; set; }
       
    }
}