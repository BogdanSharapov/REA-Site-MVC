using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Data;

namespace REASite.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly ILogger<ApartmentController> _logger;
        private readonly REASiteDbContext _context;

        public ApartmentController(REASiteDbContext context, ILogger<ApartmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ActionResult Index(int id)
        {
            var apartment = _context.Apartments
        .Include(a => a.Address)
        .Include(a => a.Images)
        .FirstOrDefault(a => a.Id == id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

    }
}
