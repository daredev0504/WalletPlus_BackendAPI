using System;

namespace WalletPlusIncAPI.Models.Dtos.Wallet
{
    public class WalletUpdateDto : WalletCreateDto
    {
        public Guid WalletId { get; set; }
    }
}