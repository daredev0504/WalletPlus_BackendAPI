using System;
using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Dtos.Funding
{
    public class ApproveFundingDto
    {
        [Required]
        public Guid FundingId { get; set; }
    }
}