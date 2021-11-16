using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WalletPlusIncAPI.Helpers.ImageService
{
    public class AddImageDto
    {
        public IFormFile Image { get; set; }
    }
}
