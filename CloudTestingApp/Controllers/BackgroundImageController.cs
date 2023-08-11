using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace CloudTestingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackgroundImageController : Controller
    {
        [HttpPost]
        public async Task<string> UploadBlobWithYourText(string text)
        {
            var blobServiceClient = new BlobServiceClient(
                new Uri("https://ivankova1.blob.core.windows.net"),
                new DefaultAzureCredential());

            string containerName = "images";

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "data";
            Directory.CreateDirectory(localPath);
            string fileName = "usertext" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Write text to the file
            await System.IO.File.WriteAllTextAsync(localFilePath, text);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Upload data from the local file
            await blobClient.UploadAsync(localFilePath, true);

            return String.Format("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
        }
    }
}
