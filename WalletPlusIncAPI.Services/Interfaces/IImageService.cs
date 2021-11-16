using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IImageService
    {
        Task<UploadResult> UploadImageAsync(IFormFile model);
    }
}
