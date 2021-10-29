using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlusIncAPI.Helpers
{
    public static class LimitCalc
    {
        public static decimal LimitForPoint{ get; set; } = 5000;
        public static decimal LimitForDeposit{ get; set; } = 1000000;
       
    }
}
