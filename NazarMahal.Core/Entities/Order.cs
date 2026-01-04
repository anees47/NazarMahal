using NazarMahal.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NazarMahal.Core.Entities;

public class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus OrderStatus { get; set; }

    public DateOnly OrderCreatedDate { get; set; }
    public TimeOnly OrderCreatedTime { get; set; }

    [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    public string? UserEmail { get; set; }

    [MaxLength(50)]
    public string OrderNumber { get; set; } = null!;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    // ONE Order → MANY OrderItems
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public Order() { }

    public Order(int userId, decimal totalAmount, string? phoneNumber, string? userEmail, string orderNumber, string? firstName, string? lastName, string? paymentMethod)
    {
        UserId = userId;
        TotalAmount = totalAmount;
        PhoneNumber = phoneNumber;
        UserEmail = userEmail;
        OrderNumber = orderNumber;
        FirstName = firstName;
        LastName = lastName;
        PaymentMethod = paymentMethod;

        OrderStatus = OrderStatus.New;
        OrderCreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        OrderCreatedTime = TimeOnly.FromDateTime(DateTime.UtcNow);
    }
}
