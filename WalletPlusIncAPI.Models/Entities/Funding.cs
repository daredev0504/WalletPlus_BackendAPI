using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletPlusIncAPI.Models.Entities
{
   
    public class Funding : BaseEntity
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsApproved { get; set; } = false;

        //navigational properties

        public int? CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }

        [ForeignKey("DestinationId")]
        public Wallet Destination { get; set; }

        public Guid DestinationId { get; set; }

    }
}