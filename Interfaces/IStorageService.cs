using System.Threading.Tasks;

namespace SampleCollector.Interfaces
{
    public interface IStorageService
    {
        Task<bool> UploadExistingFileAsync(string filePath, string fileName);

        Task<bool> UploadFromMemoryAsync(byte[] binaryData, string fileName);
    }
}