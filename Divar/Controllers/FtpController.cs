using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Security;
using Divar.Models; // برای استفاده از FtpRepository

namespace Divar.Controllers
{
    public class FtpController : Controller
    {
        private readonly IFtpRepository _ftpRepository;

        public FtpController(IFtpRepository ftpRepository)
        {
            _ftpRepository = ftpRepository;
        }

        // اکشن برای نمایش تمامی عکس‌های داخل سرور FTP
        public async Task<IActionResult> ListImages()
        {
            List<string> fileList = new List<string>();

            try
            {
                fileList = await _ftpRepository.ListImagesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در دریافت لیست فایل‌ها: {ex.Message}");
            }

            return View(fileList);
        }

        public async Task<IActionResult> GetImage(string fileName)
        {
            try
            {
                byte[] imageData = await _ftpRepository.GetImageAsync(fileName);
                return File(imageData, "image/jpeg"); // یا "image/png" بسته به نوع فایل
            }
            catch (WebException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "لطفاً فایلی برای آپلود انتخاب کنید.");
                return View("Upload");
            }

            try
            {
                await _ftpRepository.UploadFileAsync(file);
                ViewBag.Message = "فایل با موفقیت آپلود شد!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در آپلود فایل: {ex.Message}");
            }

            return View("Upload");
        }

        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                await _ftpRepository.DeleteFileAsync(fileName);
                ViewBag.Message = "فایل با موفقیت حذف شد!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در حذف فایل: {ex.Message}");
            }

            return RedirectToAction("ListImages");
        }

        [HttpPost]
        public async Task<IActionResult> EditImage(string oldFileName, IFormFile newFile)
        {
            if (newFile == null || newFile.Length == 0)
            {
                ModelState.AddModelError("", "لطفاً یک فایل جدید انتخاب کنید.");
                return View(oldFileName);
            }

            try
            {
                await _ftpRepository.EditFileAsync(oldFileName, newFile);
                ViewBag.Message = "فایل با موفقیت ویرایش شد!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در ویرایش فایل: {ex.Message}");
            }

            return RedirectToAction("ListImages");
        }
    }
}
