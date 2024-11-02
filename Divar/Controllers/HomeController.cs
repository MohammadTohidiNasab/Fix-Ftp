namespace Divar.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdvertisementRepository _adRepository;
        private readonly int pageSize = 8;

        public HomeController(IAdvertisementRepository adRepository)
        {
            _adRepository = adRepository;
        }

        // Show all advertisements
        public async Task<IActionResult> Index(int pageNumber = 1, CategoryType? category = null, string searchTerm = "")
        {
            var totalAds = await _adRepository.GetTotalAdvertisementsCountAsync(category, searchTerm);
            var totalPages = (int)Math.Ceiling((double)totalAds / pageSize);
            var ads = await _adRepository.GetAllAdvertisementsAsync(pageNumber, pageSize, category, searchTerm);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearchTerm = searchTerm;

            return View(ads);
        }

        // Show details
        public async Task<IActionResult> Detail(int id)
        {
            var ad = await _adRepository.GetAdvertisementByIdAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        // To create Advertisement
        [HttpGet]
        public IActionResult Create()
        {
            var selectedCategory = HttpContext.Session.GetString("SelectedCategory");
            if (string.IsNullOrEmpty(selectedCategory))
            {
                return RedirectToAction("SelectCategory");
            }

            var category = Enum.Parse<CategoryType>(selectedCategory);
            var model = new Advertisement { Category = category };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Advertisement advertisement)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                advertisement.UserId = userId;
                advertisement.CreatedDate = DateTime.Now;
                await _adRepository.AddAdvertisementAsync(advertisement);

                // پاک کردن سشن بعد از ذخیره آگهی
                HttpContext.Session.Remove("SelectedCategory");

                return RedirectToAction("Index");
            }
            return View(advertisement);
        }

        [HttpGet]
        public IActionResult SelectCategory()
        {
            var categories = Enum.GetValues(typeof(CategoryType)).Cast<CategoryType>();
            return View(categories);
        }

        [HttpGet]
        public IActionResult SetCategory(CategoryType category)
        {
            HttpContext.Session.SetString("SelectedCategory", category.ToString());
            return RedirectToAction("Create");
        }


        // Edit Advertisement
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Advertisement updatedAdvertisement)
        {
            if (ModelState.IsValid)
            {
                var ad = await _adRepository.GetAdvertisementByIdAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }
                ad.Title = updatedAdvertisement.Title;
                ad.Content = updatedAdvertisement.Content;
                ad.Price = updatedAdvertisement.Price;
                ad.ImageUrl = updatedAdvertisement.ImageUrl;
                //کاستوم ها
                ad.SimCardsNumber = updatedAdvertisement.SimCardsNumber;
                ad.MobileBrand = updatedAdvertisement.MobileBrand;
                ad.BookAuthor = updatedAdvertisement.BookAuthor;
                ad.CarBrand = updatedAdvertisement.CarBrand;
                ad.GearboxType = updatedAdvertisement.GearboxType;
                ad.HomeAddress = updatedAdvertisement.HomeAddress;
                ad.HomeSize = updatedAdvertisement.HomeSize;


                await _adRepository.UpdateAdvertisementAsync(ad);
                return RedirectToAction(nameof(Index));
            }
            return View(updatedAdvertisement);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ad = await _adRepository.GetAdvertisementByIdAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        // Delete advertisements
        public async Task<IActionResult> Delete(int id)
        {
            var ad = await _adRepository.GetAdvertisementByIdAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // Delete Confirm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _adRepository.DeleteAdvertisementAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // User dashboard 
        public async Task<IActionResult> Dashboard(int pageNumber = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var totalAds = await _adRepository.GetTotalAdvertisementsCountByUserIdAsync(userId.Value);
            var totalPages = (int)Math.Ceiling((double)totalAds / pageSize);

            var ads = await _adRepository.GetAdvertisementsByUserIdAsync(userId.Value, pageNumber, pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(ads);
        }

    }
}
