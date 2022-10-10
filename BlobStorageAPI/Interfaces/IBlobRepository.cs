using Azure.Storage.Blobs.Models;
using BlobStorageAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorageAPI.Interfaces
{
    public interface IBlobRepository
    {
        Task<BlobObject> GetBlobFile(string url);
        Task<string> UploadBlobFile(string filePath, string filename);
        void DeleteBlob(string path);
        Task<List<string>> ListBlobs();

    }
}
