using Microsoft.AspNetCore.Mvc;
using RestaurantMS_test.Models;

namespace RestaurantMS_test.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            DataRepository.ResetAvailabilityIfMidnight();

            var categoriesWithItems = DataRepository.Categories
                .Where(c => c.IsActive)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    MenuItems = DataRepository.MenuItems
                        .Where(m => m.CategoryId == c.Id && m.IsAvailable && m.Price > 0)
                        .ToList()
                })
                .Where(c => c.MenuItems.Any())
                .ToList();

            return View(categoriesWithItems);
        }
    }
}
