using API.Helpers;
using API.Interfaces;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Supabase.Storage;
namespace API.Services;
// Angular requests doc
//   -> .NET API checks UserClientAccess (is this user active for this client?)
//   -> IDocumentAccessService.CanViewAsync() passes
//   -> API calls GeneratePresignedUrlAsync() -> returns a 15-min URL
//   -> Angular uses that URL to render/download

public class SupabaseStorageService(IOptions<SupabaseSettings> options, Supabase.Client sbClient) : IDocumentStorageService
{ 
    private readonly SupabaseSettings _settings = options.Value; 
    //or use IConfiguration config  => config["SupabaseSettings:BucketName"];


    public async Task DeleteAsync(string storageKey)
    {
         await sbClient.Storage
            .From(_settings.BucketName)
            .Remove([storageKey]); //new List<string> { storageKey } -> simplfied to ->  [storageKey]
    }

    public async Task<string> GeneratePresignedUrlAsync(string storageKey, int expiryMinutes = 15)
    {
        var url = await sbClient.Storage.From(_settings.BucketName).CreateSignedUrl(storageKey, expiryMinutes);
        return url;
    }

    public async Task<FileUploadResult> UploadAsync(IFormFile file, string fileName, string mimeType, string storageKey)
    {
        string bucketName = _settings.BucketName ?? throw new InvalidOperationException("BucketName configuration is missing.");
        var bucket = await sbClient.Storage.GetBucket(bucketName);

        if (bucket == null)
        {
            var resultCreateBucket = await sbClient.Storage.CreateBucket(bucketName,
            new BucketUpsertOptions { Public = false });
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream); // passs it to as Upload(memoryStream.ToArray(), fileName);

        var result = await sbClient.Storage.From(bucketName).Upload(memoryStream.ToArray(), storageKey,
        new Supabase.Storage.FileOptions
        {
            ContentType = mimeType,
            Upsert = false
        });

        return new FileUploadResult
        {
            IsSuccess = true,
            Message = storageKey
        };
    }

}
