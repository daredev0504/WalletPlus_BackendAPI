using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.Wallet
{
    public class FundPremiumDto
    {
        [Required]
        public string WalletOwnerId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}