using System.ComponentModel.DataAnnotations;

namespace WalletPlusIncAPI.Models.Entities
{
   public class Currency
    {
      
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Code { get; set; }
    }
}