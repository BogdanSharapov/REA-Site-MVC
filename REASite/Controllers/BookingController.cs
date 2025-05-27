using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Data;
using REASite.Models;
using REASite.ViewModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
public class BookingController: Controller  
{  
    private readonly REASiteDbContext _context;

public BookingController(REASiteDbContext context)
{
    _context = context;
}

[HttpGet]
public IActionResult Book(int apartmentId)
{
    var apartment = _context.Apartments.Find(apartmentId);
    if (apartment == null || apartment.OfferType != OfferTypeEnum.DailyRent)
    {
        return NotFound("Квартира не найдена или неподходящего типа.");
    }

        var model = new BookingViewModel
        {
            ApartmentId = apartmentId,
            ApartmentTitle = apartment.Title,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1)
        };
    return View(model);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var startDateUtc = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
        var endDateUtc = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc);

        if (startDateUtc >= endDateUtc)
        {
            ModelState.AddModelError("", "Дата начала должна быть раньше даты окончания.");
            return View(model);
        }

        if (!await IsApartmentAvailable(model.ApartmentId, startDateUtc, endDateUtc))
        {
            ModelState.AddModelError("", "Квартира недоступна для выбранных дат.");
            return View(model);
        }

        var booking = new Booking
        {
            ApartmentId = model.ApartmentId,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            StartDate = startDateUtc,
            EndDate = endDateUtc,
            Status = "Pending"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    private async Task<bool> IsApartmentAvailable(int apartmentId, DateTime startDate, DateTime endDate)
    {
        return !await _context.Bookings.AnyAsync(b =>
                 b.ApartmentId == apartmentId &&
                 b.Status != "Cancelled" &&
                 b.StartDate < endDate &&
                 b.EndDate > startDate);
    }
}