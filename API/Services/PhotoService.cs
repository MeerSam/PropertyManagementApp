
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

class PhotoService: IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly ITenantService _tenant;
    public PhotoService(IOptions<CloudinarySettings> config, ITenantService tenant)
    {
        var account = new Account
        (config.Value.CloudName, 
         config.Value.ApiKey, 
         config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
        _tenant = tenant;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
         var deleteParams = new DeletionParams(publicId);
         return await _cloudinary.DestroyAsync(deleteParams) ;
    }

    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
    {
        var uploadResults  = new ImageUploadResult();
        if (file.Length> 0) {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder= _tenant.GetCurrentClientId() +  "-profile"

            };
            uploadResults =  await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResults;
    }
}