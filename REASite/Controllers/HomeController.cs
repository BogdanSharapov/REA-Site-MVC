using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Data;
using REASite.Models;
using ImageStorage;
using Microsoft.AspNetCore.Identity;
using REASite.Areas.Identity.Data;

namespace REASite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly REASiteDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<SiteUser> _userManager;

        public HomeController(REASiteDbContext context, ILogger<HomeController> logger, IImageService imageService, UserManager<SiteUser> userManager)
        {
            _context = context;
            _logger = logger;
            _imageService = imageService;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index(string search, string city, int? minPrice, int? maxPrice,  string offerType, string type = "Rent")
        {
            if (!Enum.TryParse<OfferTypeEnum>(type, true, out var pageType))
            {
                pageType = OfferTypeEnum.Rent;
                type = "Rent";
            }
            var apartments = _context.Apartments
                .Include(a => a.Address)
                .Include(a => a.ApartmentComforts)
                .Include(a => a.Images)
                .Where(a => a.OfferType == pageType &&
                           a.Images != null && 
                           a.Address != null) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                apartments = apartments.Where(a => EF.Functions.ILike(a.Title, $"%{search}%") ||
                                                     EF.Functions.ILike(a.Description, $"%{search}%"));
            }

            if (!string.IsNullOrEmpty(city))
            {
                apartments = apartments.Where(a => a.Address.City.Contains(city));
            }

            if (minPrice.HasValue)
            {
                apartments = apartments.Where(a => a.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                apartments = apartments.Where(a => a.Price <= maxPrice.Value);
            }
            if (!string.IsNullOrEmpty(offerType) && Enum.TryParse<OfferTypeEnum>(offerType, out var offerTypeEnum))
            {
                apartments = apartments.Where(a => a.OfferType == offerTypeEnum);
            }

            ViewBag.Type = type;
            return View (apartments);
        }
        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] int apartmentId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "��������� �����������" }); // ������ JSON

                var apartmentExists = await _context.Apartments.AnyAsync(a => a.Id == apartmentId);
                if (!apartmentExists)
                    return Json(new { success = false, message = "�������� �� �������" }); // JSON

                // ... ��������� ��� ...

                return Json(new { success = true, message = "��������� � ���������" }); // ����� JSON
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); // ������ JSON
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites([FromBody] int apartmentId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized(new { message = "��������� �����������" });

                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == user.Id && f.ApartmentId == apartmentId);

                if (favorite != null)
                {
                    _context.Favorites.Remove(favorite);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "������� �� ����������" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ �������� �� ����������");
                return StatusCode(500, new { message = "���������� ������ �������" });
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Create method!");
            return View(new Apartment
            {
                Address = new Address(),
                OfferType = OfferTypeEnum.Rent,
                ApartmentComforts = new List<ApartmentComfort>(),
                Images = new List<ApartmentImage>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Apartment apartment, List<IFormFile> images, int[] Comforts)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Normalize address parts for comparison
                    var normalizedCountry = (apartment.Address.Country ?? string.Empty).Trim().ToLower();
                    var normalizedCity = (apartment.Address.City ?? string.Empty).Trim().ToLower();
                    var normalizedStreet = (apartment.Address.Street ?? string.Empty).Trim().ToLower();
                    var normalizedHouseNum = (apartment.Address.HouseNum ?? string.Empty).Trim().ToLower();

                    // ���������, ���������� �� �����
                    var existingAddress = await _context.Addresses
                       .FirstOrDefaultAsync(a =>
                           (a.Country ?? string.Empty).Trim().ToLower() == normalizedCountry &&
                           (a.City ?? string.Empty).Trim().ToLower() == normalizedCity &&
                           (a.Street ?? string.Empty).Trim().ToLower() == normalizedStreet &&
                           (a.HouseNum ?? string.Empty).Trim().ToLower() == normalizedHouseNum);

                    if (existingAddress != null)
                    {
                        apartment.AddressId = existingAddress.Id;
                        apartment.Address = existingAddress;
                    }
                    else
                    {
                        // ��������� �������� �������� ������
                        _context.Addresses.Add(apartment.Address);
                        await _context.SaveChangesAsync(); // Save new address
                        apartment.AddressId = apartment.Address.Id;
                    }

                    if (images != null && images.Count > 0)
                    {
                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                using (var stream = image.OpenReadStream())
                                {
                                    // ��������� � LiteDB � �������� ID
                                    var imageId = await _imageService.SaveImageAsync(stream, image.FileName, image.ContentType);

                                    // ��������� ID � PostgreSQL
                                    apartment.Images.Add(new ApartmentImage
                                    {
                                        ImageID = imageId
                                    });
                                }
                            }
                        }
                    }

                    if (Comforts != null && Comforts.Length > 0)
                    {
                        apartment.ApartmentComforts = Comforts.Select(c => new ApartmentComfort { Comfort = (ComfortEnum)c }).ToList();
                    }

                    _context.Apartments.Add(apartment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "������ ��� ���������� �������� ��� �������� �����������");
                    ModelState.AddModelError("", "��������� ������ ��� ����������. ���������� �����.");
                }
            }
            return View(apartment);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Apartment? apartment = await _context.Apartments
                .Include(a => a.Address)
                .Include(a => a.ApartmentComforts)
                .Include(a => a.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Apartment apartment, List<IFormFile> images, int[] Comforts)
        {
            if (id != apartment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingApartment = await _context.Apartments
                       .Include(a => a.Address)
                       .Include(a => a.ApartmentComforts)
                       .FirstOrDefaultAsync(a => a.Id == id);
                    if (existingApartment == null)
                    {
                        return NotFound();
                    }

                    // ����������� ����� ������
                    var normalizedCountry = (apartment.Address.Country ?? string.Empty).Trim().ToLower();
                    var normalizedCity = (apartment.Address.City ?? string.Empty).Trim().ToLower();
                    var normalizedStreet = (apartment.Address.Street ?? string.Empty).Trim().ToLower();
                    var normalizedHouseNum = (apartment.Address.HouseNum ?? string.Empty).Trim().ToLower();

                    // ���������, ���������� �� �����
                    var existingAddress = await _context.Addresses
                        .FirstOrDefaultAsync(a =>
                            (a.Country ?? string.Empty).Trim().ToLower() == normalizedCountry &&
                            (a.City ?? string.Empty).Trim().ToLower() == normalizedCity &&
                            (a.Street ?? string.Empty).Trim().ToLower() == normalizedStreet &&
                            (a.HouseNum ?? string.Empty).Trim().ToLower() == normalizedHouseNum);

                    var oldAddressId = existingApartment.AddressId;

                    if (existingAddress != null)
                    {
                        existingApartment.AddressId = existingAddress.Id;
                        existingApartment.Address = existingAddress;
                    }
                    else
                    {
                        // ������ ����� ����� � ��������� ����������
                        var newAddress = new Address
                        {
                            Country = apartment.Address.Country,
                            City = apartment.Address.City,
                            Street = apartment.Address.Street,
                            HouseNum = apartment.Address.HouseNum
                        };
                        _context.Addresses.Add(newAddress);
                        await _context.SaveChangesAsync(); // ��������� ����� �����
                        existingApartment.AddressId = newAddress.Id;
                        existingApartment.Address = newAddress;
                    }

                    //��������� �����������
                    // ���������� ����� �����������
                    if (images != null && images.Count > 0)
                    {
                        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(imagesPath))
                        {
                            Directory.CreateDirectory(imagesPath);
                        }

                        foreach (var image in images)
                        {
                            using (var stream = image.OpenReadStream())
                            {
                                var imageId = await _imageService.SaveImageAsync(stream, image.FileName, image.ContentType);
                                _context.ApartmentImages.Add(new ApartmentImage
                                {
                                    ApartmentId = apartment.Id,
                                    ImageID = imageId
                                });
                            }
                        }
                    }

                    //��������� �������
                    existingApartment = await _context.Apartments
                        .Include(a => a.ApartmentComforts)
                        .FirstOrDefaultAsync(a => a.Id == id);
                    if (existingApartment != null)
                    {
                        // ���������� ������� ���������
                        var currentComforts = existingApartment.ApartmentComforts.Select(ac => ac.Comfort).ToList();
                        var newComforts = Comforts?.Select(c => (ComfortEnum)c).ToList() ?? new List<ComfortEnum>();

                        // ������� ��������, ������� ������ �� �������
                        var comfortsToRemove = existingApartment.ApartmentComforts
                            .Where(ac => !newComforts.Contains(ac.Comfort))
                            .ToList();
                        foreach (var comfort in comfortsToRemove)
                        {
                            existingApartment.ApartmentComforts.Remove(comfort);
                        }

                        // ��������� ����� ��������
                        foreach (var comfort in newComforts)
                        {
                            if (!currentComforts.Contains(comfort))
                            {
                                existingApartment.ApartmentComforts.Add(new ApartmentComfort { Comfort = comfort });
                            }
                        }

                        // ���������� ��������� �����
                        existingApartment.Title = apartment.Title;
                        existingApartment.Description = apartment.Description;
                        existingApartment.Price = apartment.Price;
                        existingApartment.Floor = apartment.Floor;
                        existingApartment.Area = apartment.Area;
                        existingApartment.RoomsCount = apartment.RoomsCount;
                        existingApartment.OfferType = apartment.OfferType;
                        
                        existingApartment.Address.Country = apartment.Address.Country;
                        existingApartment.Address.City = apartment.Address.City;
                        existingApartment.Address.Street = apartment.Address.Street;
                        existingApartment.Address.HouseNum = apartment.Address.HouseNum;

                    }

                    await _context.SaveChangesAsync();
                    await DeleteAddressIfNotLinkedToAppartmentAsync(oldAddressId);
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Apartments.Any(e => e.Id == apartment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "������ ��� ���������� �������� ��� �������� �����������");
                    ModelState.AddModelError("", "��������� ������ ��� ����������. ���������� �����.");
                }
            }

            // ���� ������ �� �������, ��������� ������� ��������
            var currentApartment = await _context.Apartments
                .Include(a => a.Address)
                .Include(a => a.ApartmentComforts)
                .FirstOrDefaultAsync(m => m.Id == id);
            return View(currentApartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteApartmentRequestModel model)
        {
            if (model == null || model.ApartmentId <= 0)
            {
                return Json(new { success = false, message = "�������� ������" });
            }

            var apartment = await _context.Apartments.FirstOrDefaultAsync(a => a.Id == model.ApartmentId);
            if (apartment == null)
            {
                return Json(new { success = false, message = "�������� �� �������" });
            }
            var addressId = apartment.AddressId;

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            // ���������, �������� �� ������ �������� � ���� �������
            await DeleteAddressIfNotLinkedToAppartmentAsync(addressId);
            return Json(new { success = true });
        }
        private async Task DeleteAddressIfNotLinkedToAppartmentAsync(int addressId)
        {
            var isAddressStillUsed = await _context.Apartments.AnyAsync(a => a.AddressId == addressId);
            if (!isAddressStillUsed && addressId != 0)
            {
                var address = await _context.Addresses.FindAsync(addressId);
                if (address != null)
                {
                    _context.Addresses.Remove(address);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteImageRequest request)
        {
            try
            {
                // ������� ����� ����������� � ���������
                var apartmentImage = await _context.ApartmentImages
                    .FirstOrDefaultAsync(ai => ai.ImageID == request.ImageId && ai.ApartmentId == request.ApartmentId);

                if (apartmentImage == null)
                    return NotFound();

                // ������� �����
                _context.ApartmentImages.Remove(apartmentImage);
                await _context.SaveChangesAsync();

                // ���������, ������������ �� ����������� � ������ ���������
                var isUsedElsewhere = await _context.ApartmentImages
                    .AnyAsync(ai => ai.ImageID == request.ImageId);

                // ���� �� ������������ - ������� �� LiteDB
                if (!isUsedElsewhere)
                {
                    await _imageService.DeleteImageAsync(request.ImageId);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� �������� �����������");
                return StatusCode(500, "������ ��� �������� �����������");
            }
        }
        public class DeleteImageRequest
        {
            public string ImageId { get; set; }
            public int ApartmentId { get; set; }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
    public class DeleteApartmentRequestModel
    {
        public int ApartmentId { get; set; }
    }

}
