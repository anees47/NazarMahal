using System.ComponentModel.DataAnnotations;

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
        
        public virtual Order Order { get; set; }
        public virtual Glasses Glasses { get; set; }

        public OrderItem() { }

        public OrderItem(int glassesId, int quantity, decimal unitPrice)
        {
            GlassesId = glassesId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalAmount = quantity * unitPrice;
        }
    }
}

