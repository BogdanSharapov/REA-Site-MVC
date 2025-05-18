using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Data;
using REASite.Models;

namespace REASite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly REASiteDbContext _context;

        public HomeController(REASiteDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            List<Apartment> ApartmentsList = _context.Apartments
                .Include(a => a.Address)
                .Include(a => a.ApartmentComforts)
                .Include(a => a.Images)
                .ToList();
            return View(ApartmentsList);
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
        public async Task<IActionResult> Create(Apartment apartment, IFormFile[] images, int[] Comforts)
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

                    // Проверяем, существует ли адрес
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
                        // Сохраняем исходные значения адреса
                        _context.Addresses.Add(apartment.Address);
                        await _context.SaveChangesAsync(); // Save new address
                        apartment.AddressId = apartment.Address.Id;
                    }

                    if (images != null && images.Length > 0)
                    {
                        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(imagesPath))
                        {
                            Directory.CreateDirectory(imagesPath);
                        }

                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                                var filePath = Path.Combine(imagesPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                apartment.Images.Add(new ApartmentImage { ImageURL = "/images/" + fileName });
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
                    _logger.LogError(ex, "Ошибка при сохранении квартиры или загрузке изображения");
                    ModelState.AddModelError("", "Произошла ошибка при сохранении. Попробуйте снова.");
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
        public async Task<IActionResult> Edit(int id, Apartment apartment, IFormFile[] images, int[] Comforts)
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

                    // Нормализуем части адреса
                    var normalizedCountry = (apartment.Address.Country ?? string.Empty).Trim().ToLower();
                    var normalizedCity = (apartment.Address.City ?? string.Empty).Trim().ToLower();
                    var normalizedStreet = (apartment.Address.Street ?? string.Empty).Trim().ToLower();
                    var normalizedHouseNum = (apartment.Address.HouseNum ?? string.Empty).Trim().ToLower();

                    // Проверяем, существует ли адрес
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
                        // Создаём новый адрес с исходными значениями
                        var newAddress = new Address
                        {
                            Country = apartment.Address.Country,
                            City = apartment.Address.City,
                            Street = apartment.Address.Street,
                            HouseNum = apartment.Address.HouseNum
                        };
                        _context.Addresses.Add(newAddress);
                        await _context.SaveChangesAsync(); // Сохраняем новый адрес
                        existingApartment.AddressId = newAddress.Id;
                        existingApartment.Address = newAddress;
                    }

                    //Обработка изображений
                    // Добавление новых изображений
                    if (images != null && images.Length > 0)
                    {
                        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(imagesPath))
                        {
                            Directory.CreateDirectory(imagesPath);
                        }

                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                                var filePath = Path.Combine(imagesPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                existingApartment.Images.Add(new ApartmentImage { ImageURL = "/images/" + fileName });
                            }
                        }
                    }

                    //Обработка удобств
                    existingApartment = await _context.Apartments
                        .Include(a => a.ApartmentComforts)
                        .FirstOrDefaultAsync(a => a.Id == id);
                    if (existingApartment != null)
                    {
                        // Обновление удобств выборочно
                        var currentComforts = existingApartment.ApartmentComforts.Select(ac => ac.Comfort).ToList();
                        var newComforts = Comforts?.Select(c => (ComfortEnum)c).ToList() ?? new List<ComfortEnum>();

                        // Удаляем удобства, которые больше не выбраны
                        var comfortsToRemove = existingApartment.ApartmentComforts
                            .Where(ac => !newComforts.Contains(ac.Comfort))
                            .ToList();
                        foreach (var comfort in comfortsToRemove)
                        {
                            existingApartment.ApartmentComforts.Remove(comfort);
                        }

                        // Добавляем новые удобства
                        foreach (var comfort in newComforts)
                        {
                            if (!currentComforts.Contains(comfort))
                            {
                                existingApartment.ApartmentComforts.Add(new ApartmentComfort { Comfort = comfort });
                            }
                        }

                        // Обновление остальных полей
                        existingApartment.Title = apartment.Title;
                        existingApartment.Description = apartment.Description;
                        existingApartment.Price = apartment.Price;
                        existingApartment.Floor = apartment.Floor;
                        existingApartment.Area = apartment.Area;
                        existingApartment.RoomsCount = apartment.RoomsCount;
                        existingApartment.OfferType = apartment.OfferType;
                        existingApartment.isFavorite = apartment.isFavorite;
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
                    _logger.LogError(ex, "Ошибка при обновлении квартиры или загрузке изображения");
                    ModelState.AddModelError("", "Произошла ошибка при обновлении. Попробуйте снова.");
                }
            }

            // Если модель не валидна, загружаем текущую квартиру
            var currentApartment = await _context.Apartments
                .Include(a => a.Address)
                .Include(a => a.ApartmentComforts)
                .FirstOrDefaultAsync(m => m.Id == id);
            return View(currentApartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var apartment = await _context.Apartments.FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }
            var addressId = apartment.AddressId;

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            // Проверяем, остались ли другие квартиры с этим адресом
            await DeleteAddressIfNotLinkedToAppartmentAsync(addressId);
            return Ok();
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
        public async Task<IActionResult> DeleteImage(int imageId, int apartmentId)
        {
            var image = await _context.ApartmentImages.FindAsync(imageId);
            if (image == null || image.ApartmentId != apartmentId)
            {
                return NotFound();
            }

            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageURL.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.ApartmentImages.Remove(image);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Удалено изображение с ID {ImageId} для квартиры ID {ApartmentId}", imageId, apartmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении изображения с ID {ImageId} для квартиры ID {ApartmentId}", imageId, apartmentId);
                return StatusCode(500, "Произошла ошибка при удалении изображения.");
            }

            return RedirectToAction("Edit", new { id = apartmentId });
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
}
