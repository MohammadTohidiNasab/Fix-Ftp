using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Divar.Models
{
    public class FtpRepository : IFtpRepository
    {
        private readonly string ftpServerUrl = "ftp://127.0.0.1/";
        private readonly string ftpUsername = "mohamad";
        private readonly string ftpPassword = "12345";

        public async Task<List<string>> ListImagesAsync()
        {
            List<string> fileList = new List<string>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUrl);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.EnableSsl = true;

            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        line.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        line.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        line.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                    {
                        fileList.Add(line);
                    }
                }
            }

            return fileList;
        }

        public async Task<byte[]> GetImageAsync(string fileName)
        {
            string ftpFullPath = ftpServerUrl + fileName;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFullPath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.EnableSsl = true;

            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            using (Stream ftpStream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                await ftpStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            string fileName = file.FileName;
            string ftpUrl = ftpServerUrl + fileName;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.EnableSsl = true;

            using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                await file.CopyToAsync(requestStream);
            }

            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            string ftpFullPath = ftpServerUrl + fileName;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFullPath);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.EnableSsl = true;

            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            {
                Console.WriteLine($"Delete File Complete, status {response.StatusDescription}");
            }
        }

        public async Task EditFileAsync(string oldFileName, IFormFile newFile)
        {
            await DeleteFileAsync(oldFileName);
            await UploadFileAsync(newFile);
        }
    }
}
