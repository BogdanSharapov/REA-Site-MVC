using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Areas.Identity.Data; // Подключите пространство имен вашего SiteUser
using REASite.Data;
using REASite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace REASite.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly REASiteDbContext _context;
        private readonly UserManager<SiteUser> _userManager;

        public FavoritesController(REASiteDbContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FavoriteRequestModel model)
        {
            if (model == null || model.ApartmentId <= 0)
            {
                return Json(new { success = false, message = "Неверные данные" });
            }

            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            // Проверяем, есть ли уже такой объект в избранном
            bool exists = await _context.Favorites
                .AnyAsync(f => f.UserId == user.Id && f.ApartmentId == model.ApartmentId);
            if (exists)
            {
                return Json(new { success = false, message = "Объект уже в избранном" });
            }

            var favorite = new Favorites
            {
                UserId = user.Id,
                ApartmentId = model.ApartmentId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // Метод для удаления объекта недвижимости из избранного
        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] FavoriteRequestModel model)
        {
            if (model == null || model.ApartmentId <= 0)
            {
                return Json(new { success = false, message = "Неверные данные" });
            }

            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            // Ищем запись в избранном
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.ApartmentId == model.ApartmentId);
            if (favorite == null)
            {
                return Json(new { success = false, message = "Объект не найден в избранном" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }

    public class FavoriteRequestModel
    {
        public int ApartmentId { get; set; }
    }
}