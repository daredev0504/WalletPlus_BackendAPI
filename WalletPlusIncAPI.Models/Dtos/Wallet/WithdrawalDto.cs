using System;
using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.Wallet
{
    public class WithdrawalDto
    {
        [Required]
        public Guid WalletId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}