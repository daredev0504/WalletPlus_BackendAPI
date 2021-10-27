using System;

namespace WalletPlusIncAPI.Models.Dtos.Transaction
{
    public class TransactionReadDto
    {
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; }

        public Guid WalletId { get; set; }

        public string CurrencyCode { get; set; }

        public int CurrencyId { get; set; }
    }
}