using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Areas.Identity.Data;
using REASite.Data;
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
}