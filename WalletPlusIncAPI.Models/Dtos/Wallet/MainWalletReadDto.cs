using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlusIncAPI.Models.Dtos.Wallet
{
    public class MainWalletReadDto
    {
        public string CurrencyCode { get; set; }
        public int CurrencyId { get; set; }
        public string FiatBalance { get; set; }
        public string PointBalance { get; set; }
        public string OwnerId { get; set; }
        public bool IsMain { get; set; }
    }
}
