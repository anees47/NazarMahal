using Microsoft.EntityFrameworkCore;
using NazarMahal.Core.Common;
using NazarMahal.Infrastructure.Data;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;
using NazarMahal.Application.Interfaces.IRepository;

namespace NazarMahal.Infrastructure.Repository
{
    public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Order> AddOrder(Order order)
        {
            var pakistanDateTime = PakistanTimeHelper.Now;

            order.OrderCreatedDate = DateOnly.FromDateTime(pakistanDateTime);
            order.OrderCreatedTime = TimeOnly.FromDateTime(pakistanDateTime);
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> RetrieveAllOrders()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Glasses)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> RetrieveOpenOrders()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Glasses)
                .Where(o => o.OrderStatus == OrderStatus.New || o.OrderStatus == OrderStatus.InProgress)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> RetrieveAllCompletedOrders()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Glasses)
                .Where(o => o.OrderStatus == OrderStatus.Completed)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> RetrieveAllCancelledOrders()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Glasses)
                .Where(o => o.OrderStatus == OrderStatus.Cancelled)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByOrderId(int orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Glasses)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }

            return order;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await GetOrderByOrderId(orderId);
            order.OrderStatus = OrderStatus.Cancelled;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderStatusByOrderId(int orderId, OrderStatus newStatus)
        {
            var order = await GetOrderByOrderId(orderId);
            order.OrderStatus = newStatus;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetOrderByUserId(int userId)
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Glasses)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
