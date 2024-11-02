namespace Divar.Models
{
    public interface IFtpRepository
    {
        Task<List<string>> ListImagesAsync();
        Task<byte[]> GetImageAsync(string fileName);
        Task UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
        Task EditFileAsync(string oldFileName, IFormFile newFile);
    }
}
