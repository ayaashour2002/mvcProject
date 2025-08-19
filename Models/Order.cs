using System.ComponentModel.DataAnnotations;

namespace RestaurantMS_test.Models
{
    public enum OrderType { DineIn, Takeout, Delivery }
    public enum OrderStatus { Pending, Preparing, Ready, Delivered, Cancelled }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        public OrderType Type { get; set; }

        public string? DeliveryAddress { get; set; }

        public DateTime OrderTime { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> Items { get; set; } = new();

        public decimal TotalBeforeTax => Items.Sum(i => i.Subtotal);

        public decimal Tax => TotalBeforeTax * 0.085m;

        public decimal Discount
        {
            get
            {
                decimal discount = 0;

                if (OrderTime.Hour >= 15 && OrderTime.Hour < 17)//3pm to 5pm 
                    discount += TotalBeforeTax * 0.2m;

                // خصم الطلبات الكبيرة 
                if (TotalBeforeTax >= 100)
                    discount += TotalBeforeTax * 0.1m;

                return discount;
            }
        }

        public decimal Total => TotalBeforeTax + Tax - Discount;

        public int EstimatedDeliveryTimeMinutes =>
            Items.Any() ? Items.Max(i => i.MenuItem?.PreparationTimeMinutes ?? 0) + 30 : 30;
    }
}
