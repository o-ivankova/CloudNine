using Azure.Identity;
using Azure.Storage.Blobs;
using CloudTestingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudTestingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WallpaperController : Controller
    {
        private readonly WallpaperInfoService _wallpaperInfoService;
        private readonly BlobContainerClient _blobContainerClient;

        public WallpaperController(WallpaperInfoService wallpaperService)
        {
            _wallpaperInfoService = wallpaperService;

            var blobServiceClient = new BlobServiceClient(
               new Uri("https://ivankova1.blob.core.windows.net"),
               new DefaultAzureCredential());

            var client = blobServiceClient.GetBlobContainerClient("images");
            client.CreateIfNotExists();

            _blobContainerClient = client;
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<byte[]> Get(string userId)
        {
            var wallpaperInfo = await _wallpaperInfoService.GetItemById(userId);

            if (wallpaperInfo == null)
            {
                return null;
            }

            var blob = _blobContainerClient.GetBlobClient(wallpaperInfo.FileName);

            using var stream = await blob.OpenReadAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task Upload(string userId, IFormFile file)
        {
            using var streamReader = new StreamReader(file.OpenReadStream());
            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var blob = _blobContainerClient.GetBlobClient(fileName);
            await blob.UploadAsync(streamReader.BaseStream);

            await _wallpaperInfoService.AddOrReplaceItem(userId, fileName);

            return;
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task Delete(string userId)
        {
            var wallpaperInfo = await _wallpaperInfoService.GetItemById(userId);

            if (wallpaperInfo == null)
            {
                return;
            }

            var blob = _blobContainerClient.GetBlobClient(wallpaperInfo.FileName);
            await blob.DeleteIfExistsAsync();

            await _wallpaperInfoService.DeleteItemById(userId);

            return;
        }
    }
}
