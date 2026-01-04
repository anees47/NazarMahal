using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;

namespace NazarMahal.Application.Interfaces.IRepository
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order order);
        Task<IEnumerable<Order>> RetrieveAllOrders();
        Task<IEnumerable<Order>> RetrieveOpenOrders();
        Task<IEnumerable<Order>> RetrieveInProgressOrders();
        Task<IEnumerable<Order>> RetrieveReadyForPickupOrders();
        Task<IEnumerable<Order>> RetrieveAllCompletedOrders();
        Task<IEnumerable<Order>> RetrieveAllCancelledOrders();
        Task<Order> GetOrderByOrderId(int orderId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> UpdateOrderStatusByOrderId(int orderId, OrderStatus newStatus);
        Task CompleteAsync();
        Task<IEnumerable<Order>> GetOrderByUserId(int userId);
    }
}
