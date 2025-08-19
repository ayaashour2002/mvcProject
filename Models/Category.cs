namespace RestaurantMS_test.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public List<MenuItem> MenuItems { get; set; } = new();
    }

}
