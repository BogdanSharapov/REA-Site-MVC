using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Areas.Identity.Data;
using REASite.Data;
using System.Linq;
using REASite.Models;
using REASite.ViewModel;
using System.Threading.Tasks;

[Authorize]
public class AccountController : Controller
{
    private readonly REASiteDbContext _context;
    private readonly UserManager<SiteUser> _userManager;

    public AccountController(REASiteDbContext context, UserManager<SiteUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var favorites = await _context.Favorites
             .Include(f => f.Apartment)
             .ThenInclude(a => a.Address)
             .Where(f => f.UserId == user.Id)
             .Select(f => f.Apartment)
             .ToListAsync();

        var bookings = await _context.Bookings
            .Include(b => b.Apartment)
            .ThenInclude(a => a.Address)
            .Where(b => b.UserId == user.Id)
            .ToListAsync();

        var viewModel = new AccountViewModel
        {
            Favorites = favorites,
            Bookings = bookings
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetBooking(int bookingId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
            .Include(b => b.Apartment)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == user.Id);

        if (booking == null)
        {
            return NotFound();
        }

        var viewModel = new BookingEditViewModel
        {
            Id = booking.Id,
            ApartmentId = booking.ApartmentId,
            ApartmentTitle = booking.Apartment.Title,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate
        };

        return PartialView("_EditBookingModal", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBooking([FromBody] BookingRequestModel model)
    {
        if (model == null || model.BookingId <= 0)
        {
            return Json(new { success = false, message = "Неверные данные" });
        }

        if (model.StartDate >= model.EndDate)
        {
            return Json(new { success = false, message = "Дата начала должна быть раньше даты окончания." });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "Пользователь не найден" });
        }

        var booking = await _context.Bookings.FindAsync(model.BookingId);
        if (booking == null || booking.UserId != user.Id)
        {
            return Json(new { success = false, message = "Бронирование не найдено" });
        }

        booking.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
        booking.EndDate = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc);

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
    public class BookingRequestModel
    {
        public int BookingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}