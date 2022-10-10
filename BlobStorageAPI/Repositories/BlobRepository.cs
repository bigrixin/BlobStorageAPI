using Azure.Storage.Blobs.Models;
using BlobStorageAPI.Interfaces;
using BlobStorageAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace BlobStorageAPI.Repositories
{
    public class BlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private BlobContainerClient client;
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".PNG" };

        public BlobRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            client = _blobServiceClient.GetBlobContainerClient("blobcontaineryt");
        }
        public async void DeleteBlob(string path)
        {
            var fileName = new Uri(path).Segments.LastOrDefault();
            var blobClient = client.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<BlobObject> GetBlobFile(string url)
        {
            var fileName = new Uri(url).Segments.LastOrDefault();

            try
            {
                var blobClient = client.GetBlobClient(fileName);
                if (await blobClient.ExistsAsync())
                {
                    BlobDownloadResult content = await blobClient.DownloadContentAsync();
                    var downloadedData = content.Content.ToStream();

                    if (ImageExtensions.Contains(Path.GetExtension(fileName.ToUpperInvariant())))
                    {
                        var extension = Path.GetExtension(fileName);
                        return new BlobObject { Content = downloadedData, ContentType = "image/" + extension.Remove(0, 1) };
                    }
                    else
                    {
                        return new BlobObject { Content = downloadedData, ContentType = content.Details.ContentType };
                    }

                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<string>> ListBlobs()
        {
            List<string> lst = new List<string>();

            await foreach (var blobItem in client.GetBlobsAsync())
            {
                lst.Add(blobItem.Name);
            }

            return lst;
        }

        public async Task<string> UploadBlobFile(string filePath, string filename)
        {
            var blobClient = client.GetBlobClient(filename);
            var status = await blobClient.UploadAsync(filePath);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
