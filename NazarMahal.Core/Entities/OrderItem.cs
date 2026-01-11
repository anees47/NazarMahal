namespace NazarMahal.Core.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int GlassesId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Glasses Glasses { get; set; } = null!;

        public OrderItem() { }

        public OrderItem(int glassesId, int quantity, decimal unitPrice)
        {
            GlassesId = glassesId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalAmount = quantity * unitPrice;
        }

        public static OrderItem Create(int glassesId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }
            if (unitPrice < 0)
            {
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
            }
            return new OrderItem(glassesId, quantity, unitPrice);
        }


    }
}

