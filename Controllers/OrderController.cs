using Microsoft.AspNetCore.Mvc;
using RestaurantMS_test.Models;

namespace RestaurantMS_test.Controllers
{
    public class OrderController : Controller
    {
        // قائمة الطلبات
        public IActionResult Index()
        {
            var orders = DataRepository.Orders;
            return View(orders);
        }

        // عرض تفاصيل الطلب
        //public IActionResult Details(int id)
        //{
        //    var order = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
        //    if (order == null) return NotFound();
        //    return View(order);
        //}




        public IActionResult Details(int id)
        {
            var order = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            if (order.Type == OrderType.Delivery)
            {
                int maxPrep = order.Items.Any()
                    ? order.Items.Max(i => i.MenuItem?.PreparationTimeMinutes ?? 0)
                    : 0;

                ViewBag.EstimatedDeliveryTime = order.OrderTime.AddMinutes(maxPrep + 30);
            }


            var now = DateTime.Now;
            var elapsed = now - order.OrderTime;

            if (order.Status == OrderStatus.Pending && elapsed.TotalMinutes >= 5)
            {
                order.Status = OrderStatus.Preparing;
                ViewBag.Message = "Order status automatically moved to 'Preparing'.";
            }

            if (order.Status == OrderStatus.Preparing)
            {
                int maxPrepTime = order.Items.Any() ? order.Items.Max(i => i.MenuItem.PreparationTimeMinutes) : 10;
                if (elapsed.TotalMinutes >= (5+ maxPrepTime))
                {
                    order.Status = OrderStatus.Ready;
                    ViewBag.Message = "Order status automatically moved to 'Ready'.";
                }
            }

            return View(order);
        }


        //  (GET)
        public IActionResult Create()
        {
            ViewBag.MenuItems = DataRepository.MenuItems
        .Where(m => m.IsAvailable && DataRepository.Categories.FirstOrDefault(c => c.Id == m.CategoryId)?.IsActive == true)
        .ToList();


            return View(new Order());
        }

        // إنشاء طلب جديد (POST)
        [HttpPost]
        public IActionResult Create(Order order, int[] menuItemIds, int[] quantities)
        {
            for (int i = 0; i < menuItemIds.Length; i++)
            {
                var item = DataRepository.MenuItems.FirstOrDefault(m => m.Id == menuItemIds[i]);
                var category = DataRepository.Categories.FirstOrDefault(c => c.Id == item?.CategoryId);

                if (item == null || !item.IsAvailable || category == null || !category.IsActive)
                {
                    ModelState.AddModelError("", $"Item '{item?.Name}' is unavailable or its category is inactive.");
                    ViewBag.MenuItems = DataRepository.MenuItems
    .Where(m => m.IsAvailable && DataRepository.Categories.FirstOrDefault(c => c.Id == m.CategoryId)?.IsActive == true)
    .ToList();

                    return View(order);
                }

                if (quantities[i] > 0)
                {
                    order.Items.Add(new OrderItem
                    {
                        MenuItemId = item.Id,
                        Quantity = quantities[i]
                    });

                    item.DailyOrderCount += quantities[i];

                    if (item.DailyOrderCount > 50)
                        item.IsAvailable = false;
                }
            }

            // عنوان التوصيل
            if (order.Type == OrderType.Delivery && string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                ModelState.AddModelError("DeliveryAddress", "Delivery address is required for delivery orders.");
                ViewBag.MenuItems = DataRepository.MenuItems
                    .Where(i => i.IsAvailable && DataRepository.Categories.FirstOrDefault(c => c.Id == i.CategoryId)?.IsActive == true)
                    .ToList();
                return View(order);
            }

            order.Id = DataRepository.Orders.Count + 1;
            order.OrderTime = DateTime.Now;
            order.Status = OrderStatus.Pending;

            DataRepository.Orders.Add(order);
            TempData["Message"] = "Order placed successfully.";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();

            // فقط العناصر المتاحة 
            ViewBag.MenuItems = DataRepository.MenuItems
                .Where(m => m.IsAvailable && DataRepository.Categories
                .FirstOrDefault(c => c.Id == m.CategoryId)?.IsActive == true)
                .ToList();

            return View(order);
        }

      [HttpPost]
[ActionName("Edit")]
public IActionResult Edit(int id, Order order, int[] menuItemIds, int[] quantities)
{
    var existingOrder = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
    if (existingOrder == null)
        return NotFound();

    existingOrder.CustomerName = order.CustomerName;
    existingOrder.Type = order.Type;
    existingOrder.DeliveryAddress = order.DeliveryAddress;

    existingOrder.Items.Clear();

    for (int i = 0; i < menuItemIds.Length; i++)
    {
        var item = DataRepository.MenuItems.FirstOrDefault(m => m.Id == menuItemIds[i]);

        // تحقق من وجود الصنف وتوفره وقسمه مفعل
        var category = DataRepository.Categories.FirstOrDefault(c => c.Id == item?.CategoryId);
        if (item == null || !item.IsAvailable || category == null || !category.IsActive)
        {
            ModelState.AddModelError("", $"Item '{item?.Name}' is not available or its category is inactive.");
            ViewBag.MenuItems = DataRepository.MenuItems
                .Where(m => m.IsAvailable && DataRepository.Categories
                .FirstOrDefault(c => c.Id == m.CategoryId)?.IsActive == true)
                .ToList();
            return View(order);
        }
                // عنوان التوصيل
                if (order.Type == OrderType.Delivery && string.IsNullOrWhiteSpace(order.DeliveryAddress))
                {
                    ModelState.AddModelError("DeliveryAddress", "Delivery address is required for delivery orders.");
                    ViewBag.MenuItems = DataRepository.MenuItems
                        .Where(i => i.IsAvailable && DataRepository.Categories.FirstOrDefault(c => c.Id == i.CategoryId)?.IsActive == true)
                        .ToList();
                    return View(order);
                }

                if (quantities[i] > 0)
        {
            existingOrder.Items.Add(new OrderItem
            {
                MenuItemId = item.Id,
                Quantity = quantities[i]
            });

            item.DailyOrderCount += quantities[i];

            if (item.DailyOrderCount > 50)
                item.IsAvailable = false;
        }
    }

    // بعد التعديل،  ة
    ViewBag.MenuItems = DataRepository.MenuItems
        .Where(m => m.IsAvailable && DataRepository.Categories
        .FirstOrDefault(c => c.Id == m.CategoryId)?.IsActive == true)
        .ToList();

    return RedirectToAction("Index");
}




        [HttpPost]
        public IActionResult DeletePermanent(int id)
        {
            var order = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            if (order.Status != OrderStatus.Cancelled)
            {
                TempData["Error"] = "Only cancelled orders can be deleted permanently.";
                return RedirectToAction("Details", new { id });
            }

            DataRepository.Orders.Remove(order);
            TempData["Message"] = $"Order #{id} deleted permanently.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelOrder (int id)
        {
            var order = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();

            

            order.Status = OrderStatus.Cancelled;
            TempData["Message"] = "Order has been cancelled.";
            return RedirectToAction("Details", new { id });
        }







        // تغيير حالة الطلب
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var order = DataRepository.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            switch (order.Status)
            {
                case OrderStatus.Pending:
                    order.Status = OrderStatus.Preparing;
                    ViewBag.Message = "Kitchen marked order as 'Preparing'.";
                    break;

                case OrderStatus.Preparing:
                    order.Status = OrderStatus.Ready;
                    ViewBag.Message = "Kitchen marked order as 'Ready'.";
                    break;

                default:
                    ViewBag.Message = "Cannot update status from current state.";
                    break;
            }

            return RedirectToAction("Details", new { id });
        }

    }

}
