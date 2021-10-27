using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletPlusIncAPI.Models.Entities
{
    /// <summary>
    ///
    /// </summary>
    public class Transaction : BaseEntity
    {
        
        [Key]
        public Guid Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        public bool IsApproved { get; set; } = false;


        //navigational properties

        [ForeignKey("WalletId")]
        public Wallet Wallet { get; set; }

        [Required]
        public Guid WalletId { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }

        [Required]
        public int? CurrencyId { get; set; }
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }
}