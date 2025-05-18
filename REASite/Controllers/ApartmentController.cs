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

        // GET: PropertyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PropertyController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropertyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropertyController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PropertyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropertyController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PropertyController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
