using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletPlusIncAPI.Models.Entities
{
    
    public class Wallet : BaseEntity
    {
       
        public Guid Id { get; set; }

       public decimal Balance { get; set; }
       public bool IsMain { get; set; }



       //navigational properties

        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }

        public int CurrencyId { get; set; }

        [ForeignKey("OwnerId")]
        public AppUser Owner { get; set; }

        [Required]
        public WalletType WalletType { get; set; }

        public string OwnerId { get; set; }

        public IList<Transaction> Transactions { get; set; }
    }

    public enum WalletType
    {
        Point = 0,
        Fiat
    }
}