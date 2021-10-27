using System;
using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.Currency
{
    public class ChangeMainCurrencyDto
    {
        [Required]
        public Guid OldMainCurrencyWalletId { get; set; }

        [Required]
        public Guid NewMainCurrencyWalletId { get; set; }
    }
}