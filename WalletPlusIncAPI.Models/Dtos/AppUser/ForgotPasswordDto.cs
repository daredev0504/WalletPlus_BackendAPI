using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.AppUser
{
    public class ForgotPasswordDto
    {
         [Required]
        [EmailAddress]
         public string Email { get; set; }
         public string ClientURI { get; set; }
    }
}
