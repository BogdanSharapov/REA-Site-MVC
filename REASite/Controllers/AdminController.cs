using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REASite.Areas.Identity.Data;
using REASite.Data;
using REASite.ViewModel;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<SiteUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new List<UserRolesViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = roles.ToList()
            });
        }
        return View(userRoles);
    }

    public async Task<IActionResult> ManageRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        var roles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        var model = new ManageRolesViewModel
        {
            UserId = user.Id,
            Username = user.UserName,
            CurrentRoles = roles.ToList(),
            AllRoles = allRoles
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("ManageRoles", new { id = userId });
        }
        return RedirectToAction("ManageRoles", new { id = userId });
    }
    [HttpPost]
    public async Task<IActionResult> RemoveRole(string userId, string[] rolesToRemove)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (rolesToRemove == null || !rolesToRemove.Any())
        {
            ModelState.AddModelError("", "Не выбрано ни одной роли для удаления.");
            return View("ManageRoles", new { id = userId });
        }

        foreach (var roleName in rolesToRemove)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", $"Ошибка при удалении роли {roleName}: {error.Description}");
                    }
                }
            }
        }

        if (ModelState.ErrorCount > 0)
        {
            return View("ManageRoles", new { id = userId });
        }

        return RedirectToAction("ManageRoles", new { id = userId });
    }
}