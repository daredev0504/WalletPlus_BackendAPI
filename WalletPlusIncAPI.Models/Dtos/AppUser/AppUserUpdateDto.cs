using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.AppUser
{
    public class AppUserUpdateDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")] 
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
