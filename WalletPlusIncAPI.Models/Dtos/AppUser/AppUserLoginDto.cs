using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.AppUser
{
    public class AppUserLoginDto
    {
        [Required(ErrorMessage = "Email is required")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Password name is required")]
        public string Password { get; set; }

    }
}
