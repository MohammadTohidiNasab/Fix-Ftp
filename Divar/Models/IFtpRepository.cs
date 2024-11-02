public interface IFtpRepository
{
    Task<List<string>> ListImagesAsync();
    Task<byte[]> GetImageAsync(string fileName);
    Task UploadFileAsync(IFormFile file);
    Task DeleteImageAsync(string fileName);
    Task EditImageAsync(string oldFileName, IFormFile newFile);
}
