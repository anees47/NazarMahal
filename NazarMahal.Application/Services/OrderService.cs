using Microsoft.Extensions.Logging;
using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.Mappers;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Common;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;

namespace NazarMahal.Application.Services
{
    public class OrderService(
        IOrderRepository orderRepository,
        ILogger<OrderService> logger,
        IUserRepository userRepository,
        IGlassesService glassesService,
        IGlassesRepository glassesRepository,
        INotificationService notificationService) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IGlassesService _glassesService = glassesService;
        private readonly IGlassesRepository _glassesRepository = glassesRepository;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<ActionResponse<OrderDto>> CreateOrder(CreateOrderRequestDto orderRequest)
        {
            try
            {
                if (!ValidateOrderRequest(orderRequest, out var validationError))
                    return new FailActionResponse<OrderDto>(validationError);

                var userExist = await _userRepository.GetUserByIdAsync(orderRequest.UserId);
                if (userExist == null)
                    return new FailActionResponse<OrderDto>("User not found");

                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var item in orderRequest.OrderItems)
                {
                    var glasses = await _glassesRepository.GetGlassesById(item.GlassesId);
                    if (glasses == null)
                        return new FailActionResponse<OrderDto>($"Product with ID {item.GlassesId} not found");

                    if (glasses.AvailableQuantity < item.Quantity)
                        return new FailActionResponse<OrderDto>($"Insufficient inventory for {glasses.Name}. Only {glasses.AvailableQuantity} available.");

                    glasses.AvailableQuantity -= item.Quantity;
                    await _glassesRepository.UpdateGlasses(glasses);

                    var orderItem = OrderItem.Create(item.GlassesId, item.Quantity, glasses.Price);
                    orderItems.Add(orderItem);

                    totalAmount += orderItem.TotalAmount;
                }

                var order = new Order(orderRequest.UserId, totalAmount, orderRequest.PhoneNumber, orderRequest.UserEmail, GenerateOrderNumber(), orderRequest.FirstName, orderRequest.LastName, orderRequest.PaymentMethod);

                var createdOrder = await _orderRepository.AddOrder(order);

                foreach (var item in orderItems)
                    item.OrderId = createdOrder.OrderId;

                createdOrder.OrderItems = orderItems;
                await _orderRepository.CompleteAsync();

                var orderDto = createdOrder.ToOrderDto() ?? throw new InvalidOperationException("Failed to map order to DTO");

                try
                {
                    if (!string.IsNullOrWhiteSpace(orderRequest.UserEmail))
                        await _notificationService.SendOrderConfirmationEmail(orderRequest.UserEmail, createdOrder.OrderNumber, createdOrder.TotalAmount);

                    await _notificationService.SendAdminNewOrderEmail(createdOrder.OrderNumber, createdOrder.TotalAmount, orderRequest.UserEmail, orderRequest.PhoneNumber);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "Failed to send one or more order notification emails for order {OrderNumber}", createdOrder.OrderNumber);
                }

                return new OkActionResponse<OrderDto>(orderDto, "Order created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return new FailActionResponse<OrderDto>("An error occurred while creating the order.");
            }
        }


        private bool ValidateOrderRequest(CreateOrderRequestDto request, out string error)
        {
            error = null!;
            if (request.UserId <= 0)
            {
                error = "UserId is required.";
                return false;
            }
            if (request.OrderItems == null || !request.OrderItems.Any())
            {
                error = "At least one order item is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                error = "First name is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                error = "Last name is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            {
                error = "Amount must be greater than zero.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.UserEmail))
            {
                error = "UserEmail is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                error = "PhoneNumber is required.";
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.PhoneNumber, @"^03\d{9}$"))
            {
                error = "Phone number must be 11 digits.";
                return false;
            }

            return true;
        }

        private string GenerateOrderNumber()
        {
            return $"NM-{PakistanTimeHelper.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.RetrieveAllOrders();
                var orderDtos = orders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(orderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving all orders.");
            }
        }

        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetOpenOrders()
        {
            try
            {
                var orders = await _orderRepository.RetrieveOpenOrders();
                var orderDtos = orders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(orderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving open orders.");
            }
        }
        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetInProgressOrder()
        {
            try
            {
                var orders = await _orderRepository.RetrieveInProgressOrders();
                var orderDtos = orders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(orderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving open orders.");
            }
        }
        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetReadyForPickupOrders()
        {
            try
            {
                var orders = await _orderRepository.RetrieveReadyForPickupOrders();
                var orderDtos = orders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(orderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving open orders.");
            }
        }

        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetCompletedOrders()
        {
            try
            {
                var orders = await _orderRepository.RetrieveAllCompletedOrders();
                var orderDtos = orders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(orderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving completed orders.");
            }
        }

        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetCancelledOrders()
        {
            try
            {
                var orders = await _orderRepository.RetrieveAllCancelledOrders();
                var orderDtos = orders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(orderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving cancelled orders.");
            }
        }

        public async Task<ActionResponse<OrderResponseDto>> GetOrderByOrderId(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByOrderId(orderId);

                if (order == null)
                {
                    return new FailActionResponse<OrderResponseDto>("Order not found");
                }

                var orderDto = order.ToOrderResponseDto();
                return new OkActionResponse<OrderResponseDto>(orderDto);
            }
            catch (Exception)
            {
                return new FailActionResponse<OrderResponseDto>("An error occurred while retrieving the order.");
            }
        }

        public async Task<ActionResponse<OrderResponseDto>> CancelOrder(int orderId, bool orderStatus)
        {
            try
            {
                var order = await _orderRepository.GetOrderByOrderId(orderId);
                if (order == null)
                {
                    return new FailActionResponse<OrderResponseDto>("Order not found.");
                }

                order.OrderStatus = orderStatus ? OrderStatus.Cancelled : OrderStatus.New;
                await _orderRepository.CompleteAsync();

                var orderDto = order.ToOrderResponseDto();
                return new OkActionResponse<OrderResponseDto>(orderDto,
                    orderStatus ? "Order canceled successfully." : "Order uncancelled successfully.");
            }
            catch (Exception)
            {
                return new FailActionResponse<OrderResponseDto>("An error occurred while canceling/uncanceling the order.");
            }
        }

        public async Task<ActionResponse<OrderResponseDto>> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            try
            {
                var order = await _orderRepository.GetOrderByOrderId(orderId);
                if (order == null)
                {
                    return new FailActionResponse<OrderResponseDto>("Order not found.");
                }
                if (order.OrderStatus == newStatus)
                    return new FailActionResponse<OrderResponseDto>("New status is same as current status.");

                order.OrderStatus = newStatus;
                await _orderRepository.CompleteAsync();

                var orderDto = order.ToOrderResponseDto();

                try
                {
                    if (newStatus == OrderStatus.ReadyForPickup)
                    {
                        await _notificationService.SendOrderStatusUpdateEmail(order.UserEmail ?? string.Empty, order.OrderNumber, "Ready for pickup");
                    }
                }
                catch (Exception)
                {
                    // ignore email errors
                }

                return new OkActionResponse<OrderResponseDto>(orderDto, $"Order status updated successfully to {newStatus}.");
            }
            catch (Exception)
            {
                return new FailActionResponse<OrderResponseDto>("An error occurred while updating the order status.");
            }
        }

        public async Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetOrderByUserId(int userId)
        {
            try
            {
                var userOrders = await _orderRepository.GetOrderByUserId(userId);
                if (userOrders == null || !userOrders.Any())
                    return new FailActionResponse<IEnumerable<OrderResponseDto>>("No orders found for the user.");

                var userOrderDtos = userOrders.ToOrderResponseDtoList();
                return new OkActionResponse<IEnumerable<OrderResponseDto>>(userOrderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving the user's orders.");
            }
        }
    }
}
