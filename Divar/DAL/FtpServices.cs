using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

public class FtpService
{
    private readonly string ftpServerUrl;
    private readonly string ftpUsername;
    private readonly string ftpPassword;

    public FtpService(string serverUrl, string username, string password)
    {
        ftpServerUrl = serverUrl;
        ftpUsername = username;
        ftpPassword = password;
    }

    public async Task UploadImageAsync(string localFilePath, string imageName)
    {
        string uploadUrl = $"{ftpServerUrl}{imageName}";

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

        byte[] fileContents;

        using (StreamReader sourceStream = new StreamReader(localFilePath))
        {
            fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        }

        request.ContentLength = fileContents.Length;

        using (Stream requestStream = request.GetRequestStream())
        {
            await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
        }

        using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
        {
            Console.WriteLine($"Upload Complete, status {response.StatusDescription}");
        }
    }

    public async Task<List<string>> GetImagesAsync(int advertisementId)
    {
        // Your logic to retrieve images for a particular advertisement
        // For example, listing all files in the advertisementId's directory
        // This is just a placeholder, you'll adapt it to your logic
        var images = new List<string>();
        string directoryUrl = $"{ftpServerUrl}{advertisementId}/"; // Assuming directory for each advertisement

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryUrl);
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

        using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                images.Add(line);
            }
        }

        return images;
    }

    public async Task DeleteImageAsync(string imageName)
    {
        string deleteUrl = $"{ftpServerUrl}{imageName}";

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(deleteUrl);
        request.Method = WebRequestMethods.Ftp.DeleteFile;
        request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

        using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
        {
            Console.WriteLine($"Delete Complete, status {response.StatusDescription}");
        }
    }

    public async Task ReplaceImageAsync(string localFilePath, string imageName)
    {
        await DeleteImageAsync(imageName); // First delete the existing image
        await UploadImageAsync(localFilePath, imageName); // Upload the new image
    }
}
