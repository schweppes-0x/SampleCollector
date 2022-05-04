using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SampleCollector.Extensions;
using SampleCollector.Interfaces;
using SampleCollector.Options;

namespace SampleCollector.Services
{
    public class StorageService : IStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;

        public StorageService(IOptions<AzureStorageOptions> options)
        {
            _blobContainerClient = new BlobServiceClient(options.Value.ConnectionString).GetBlobContainerClient(options.Value.ContainerName);
        }

        public async Task<bool> UploadExistingFileAsync(string filePath, string fileName)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = filePath.GetContentType() });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UploadFromMemoryAsync(byte[] content, string fileName)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(fileName);
                await using var memoryStream = new MemoryStream(content);
                await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = fileName.GetContentType() });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}