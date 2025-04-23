using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using REASite.Data;
using REASite.Models;

namespace REASite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly REASiteDbContext _context;

        public HomeController(REASiteDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<ApartmentModel> ApartmentsList = _context.Apartments.ToList();

            return View(ApartmentsList);
        }

        public IActionResult Create()
        {

            return View();
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
