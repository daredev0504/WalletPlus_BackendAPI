using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WalletPlusIncAPI.Helpers.ImageService;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _appConfig;
        private readonly AccountSettings _accountSettings;
        private readonly Cloudinary cloudinary;

        public ImageService(IOptions<AccountSettings> accountSettings, IConfiguration configuration)
        {
            _appConfig = configuration;
            _accountSettings = accountSettings.Value;
            cloudinary = new Cloudinary(new Account(_accountSettings.CloudName, _accountSettings.ApiKey, _accountSettings.ApiSecret));
            cloudinary.Api.Secure = true;
        }

        public async Task<UploadResult> UploadImageAsync(IFormFile image)
        {

            // validate the image size and extension type using settings from appsettings
            var pictureFormat = false;
            var listOfExtensions = _appConfig.GetSection("PhotoSettings:Extensions").Get<List<string>>();

            for (int i = 0; i < listOfExtensions.Count; i++)
            {
                if (image.FileName.EndsWith(listOfExtensions[i]))
                {
                    pictureFormat = true;
                    break;
                }
            }
            if (pictureFormat == false)
                throw new Exception("File must be .jpg, .jpeg or .png");

            var pixSize = Convert.ToInt64(_appConfig.GetSection("PhotoSettings:Size").Get<string>());
            if (image == null || image.Length > pixSize)
                throw new Exception("File size should not exceed 2mb");
            if (!pictureFormat)
                throw new Exception("File format is not supported. Please upload a picture");

            //object to return
            var uploadResult = new ImageUploadResult();

            //fetch image as stream of data
            using (var imageStream = image.OpenReadStream())
            {
                string fileName = Guid.NewGuid().ToString() + "_" + image.Name;
                //upload to cloudinary
                uploadResult = await cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(fileName, imageStream),
                    Transformation = new Transformation().Crop("thumb").Gravity("face").Width(1000)
                        .Height(1000).Radius(40)

                });
            }
            return uploadResult;
        }
    }
}