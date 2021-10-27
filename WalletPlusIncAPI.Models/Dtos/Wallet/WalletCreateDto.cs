using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.Wallet
{
    public class WalletCreateDto
    {
        [Required(ErrorMessage = "Wallet currency id is required")]
        public int CurrencyId { get; set; }
    }
}