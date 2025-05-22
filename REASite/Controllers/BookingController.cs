using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Data;
using REASite.Models;
using REASite.ViewModel;
using System.Reflection;
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
        return NotFound();
    }
    var model = new BookingViewModel
    {
        ApartmentId = apartmentId,
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

        if (!IsApartmentAvailable(model.ApartmentId, startDateUtc, endDateUtc))
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

    private bool IsApartmentAvailable(int apartmentId, DateTime startDate, DateTime endDate)
{
    return !_context.Bookings
        .Where(b => b.ApartmentId == apartmentId &&
                    b.Status != "Cancelled" &&
                    (b.StartDate < endDate && b.EndDate > startDate))
        .Any();
}
}