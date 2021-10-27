using System;

namespace WalletPlusIncAPI.Models.Dtos.Funding
{
    
    public class FundingReadDto
    {
        public decimal Amount { get; set; }

        public Guid Id { get; set; }

        public Guid DestinationId { get; set; }

        public string CurrencyCode { get; set; }
    }
}