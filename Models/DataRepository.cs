namespace RestaurantMS_test.Models
{
    public static class DataRepository
    {
        public static List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public static List<Category> Categories { get; set; } = new List<Category>();
        public static List<Order> Orders { get; set; } = new();
        private static DateTime _lastResetDate = DateTime.Today;
        public static Dictionary<int, int> DailyOrderCount = new(); // 

        static DataRepository()
        {
            SeedData();
        }

        private static void SeedData()
        {
            Categories = new List<Category>
            {
                new Category { Id = 1, Name = "Pizza", IsActive = true },
                new Category { Id = 2, Name = "Drinks", IsActive = true },
                new Category { Id = 3, Name = "Desserts", IsActive = true },
            };

                        MenuItems = new List<MenuItem>
            {
                new MenuItem { Id = 1, Name = "Margherita", Price = 9.99m, PreparationTimeMinutes = 10, CategoryId = 1 },
                //new MenuItem { Id = 1, Name = "Tuna", Price = 8.65m, PreparationTimeMinutes = 10, CategoryId = 1 },
                //new MenuItem { Id = 1, Name = "Mix Cheeses", Price = 7.23m, PreparationTimeMinutes = 10, CategoryId = 1 },
                //new MenuItem { Id = 1, Name = "Vegetarainin", Price = 7.23m, PreparationTimeMinutes = 10, CategoryId = 1 },



                new MenuItem { Id = 2, Name = "Coca-Cola", Price = 5, PreparationTimeMinutes = 20, CategoryId = 2 },
                //new MenuItem { Id = 2, Name = "Fresh Juice", Price = 5, PreparationTimeMinutes = 20, CategoryId = 2 },
                //new MenuItem { Id = 2, Name = "Lemonada", Price = 5, PreparationTimeMinutes = 20, CategoryId = 2 },


                //new MenuItem { Id = 3, Name = "Cinamon Rolls", Price = 6, PreparationTimeMinutes = 15, CategoryId = 3, IsAvailable = true },
                //new MenuItem { Id = 3, Name = "Pankake", Price = 4, PreparationTimeMinutes = 15, CategoryId = 3, IsAvailable = true },
                //new MenuItem { Id = 3, Name = "Cheese Cake", Price = 7, PreparationTimeMinutes = 15, CategoryId = 3, IsAvailable = true },
                //new MenuItem { Id = 3, Name = "pudding", Price = 7.50m, PreparationTimeMinutes = 15, CategoryId = 3, IsAvailable = true },








            };



        }




        public static void ResetAvailabilityIfMidnight()
        {
            if (_lastResetDate < DateTime.Today)
            {
                foreach (var item in MenuItems)
                {
                    item.DailyOrderCount = 0;
                    item.IsAvailable = true;
                }
                _lastResetDate = DateTime.Today;
            }
        }



        public static void TrackOrder(int menuItemId, int quantity)
        {
            if (DailyOrderCount.ContainsKey(menuItemId))
                DailyOrderCount[menuItemId] += quantity;
            else
                DailyOrderCount[menuItemId] = quantity;

            if (DailyOrderCount[menuItemId] > 50)
            {
                var item = MenuItems.FirstOrDefault(i => i.Id == menuItemId);
                if (item != null) item.IsAvailable = false;
            }
        }
    }

}
