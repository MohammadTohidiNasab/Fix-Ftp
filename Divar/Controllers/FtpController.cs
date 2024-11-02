namespace Ftp_CRUD.Controllers
{
    public class FtpController : Controller
    {
        private readonly IFtpRepository _ftpRepository;

        public FtpController(IFtpRepository ftpRepository)
        {
            _ftpRepository = ftpRepository;
        }
        //          /ftp/ListImages
        public async Task<IActionResult> ListImages()
        {
            var fileList = await _ftpRepository.ListImagesAsync();
            ViewBag.FtpServerUrl = "ftp://127.0.0.1/";
            return View(fileList);
        }

        public async Task<IActionResult> GetImage(string fileName)
        {
            var fileBytes = await _ftpRepository.GetImageAsync(fileName);
            return File(fileBytes, "image/jpeg"); // یا "image/png" بسته به نوع فایل
        }


        //          /ftp/Upload
        public IActionResult Upload()
        {
            return View();
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




        //          delete images
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                await _ftpRepository.DeleteImageAsync(fileName);
                ViewBag.Message = "فایل با موفقیت حذف شد!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در حذف فایل: {ex.Message}");
            }

            return RedirectToAction("ListImages");
        }



        //          Edit images
        public IActionResult EditImage(string fileName)
        {
            ViewBag.FileName = fileName;
            return View();
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
                await _ftpRepository.EditImageAsync(oldFileName, newFile);
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
