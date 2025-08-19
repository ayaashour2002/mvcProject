namespace RestaurantMS_test.Models
{
    public class OrderItem
    {
        public int MenuItemId { get; set; }
        public MenuItem MenuItem => DataRepository.MenuItems.FirstOrDefault(m => m.Id == MenuItemId);
        public int Quantity { get; set; }

        public decimal Subtotal => MenuItem != null ? MenuItem.Price * Quantity : 0;
    }


}
