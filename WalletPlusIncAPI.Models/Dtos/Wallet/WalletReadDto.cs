using System;

namespace WalletPlusIncAPI.Models.Dtos.Wallet
{
   
    public class WalletReadDto
    {
        public Guid Id { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencyId { get; set; }

        public decimal Balance { get; set; }

        public string OwnerId { get; set; }

        public bool IsMain { get; set; }
    }
}