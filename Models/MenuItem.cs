namespace RestaurantMS_test.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public bool IsAvailable { get; set; } = true; // متاح أو لا
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // عدد الطلبات اليومية لهذا العنصر
        public int DailyOrderCount { get; set; } = 0;

        public void UpdateAvailability()
        {
            if (DailyOrderCount > 50)
                IsAvailable = false;
        }

        public void ResetAvailability()
        {
            DailyOrderCount = 0;
            IsAvailable = true;
        }
    }


}
