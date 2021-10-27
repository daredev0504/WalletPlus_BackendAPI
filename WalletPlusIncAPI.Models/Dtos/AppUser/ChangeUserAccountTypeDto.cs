using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.AppUser
{
   
    public class ChangeUserAccountTypeDto
    {
       
        [Required]
        public string NewType { get; set; }
        public string CurrentType { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}