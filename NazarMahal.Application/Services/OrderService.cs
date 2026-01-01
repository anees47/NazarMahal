using AutoMapper;
using Microsoft.Extensions.Logging;
using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Common;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;

namespace NazarMahal.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IGlassesService _glassesService;
        private readonly IGlassesRepository _glassesRepository;
        private readonly INotificationService _notificationService;

        public OrderService(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<OrderService> logger,
            IUserRepository userRepository,
            IGlassesService glassesService,
            IGlassesRepository glassesRepository,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _glassesService = glassesService;
            _glassesRepository = glassesRepository;
            _notificationService = notificationService;
        }

        public async Task<ActionResponse<OrderDto>> CreateOrder(CreateOrderRequestDto orderRequest)
        {
            // Note: Transactions should be handled through UnitOfWork pattern
            // For now, operations are done through repositories
            try
            {
                if (!ValidateOrderRequest(orderRequest, out var validationError))
                {
                    return new FailActionResponse<OrderDto>(validationError);
                }

                var userExist = await _userRepository.GetUserByIdAsync(orderRequest.UserId);
                if (userExist == null)
                {
                    return new FailActionResponse<OrderDto>("User not found");
                }

                // Validate and process all order items
                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var item in orderRequest.OrderItems)
                {
                    var glasses = await _glassesRepository.GetGlassesById(item.GlassesId);

                    if (glasses == null)
                    {
                        return new FailActionResponse<OrderDto>($"Product with ID {item.GlassesId} not found");
                    }

                    if (glasses.AvailableQuantity < item.Quantity)
                    {
                        return new FailActionResponse<OrderDto>($"Insufficient inventory for {glasses.Name}. Only {glasses.AvailableQuantity} available.");
                    }

                    // Update inventory
                    glasses.AvailableQuantity -= item.Quantity;
                    await _glassesRepository.UpdateGlasses(glasses);

                    // Create order item
                    var orderItem = new OrderItem(item.GlassesId, item.Quantity, item.UnitPrice);
                    orderItems.Add(orderItem);
                    totalAmount += orderItem.TotalAmount;
                }

                // Create order
                var order = new Order(
                    orderRequest.UserId,
                    totalAmount,
                    orderRequest.PhoneNumber,
                    orderRequest.UserEmail,
                    GenerateOrderNumber(),
                    orderRequest.FirstName,
                    orderRequest.LastName,
                    orderRequest.PaymentMethod
                );

                // Create order through repository
                var createdOrder = await _orderRepository.AddOrder(order);

                // Add order items with correct OrderId
                foreach (var item in orderItems)
                {
                    item.OrderId = createdOrder.OrderId;
                }

                // Add order items to the created order
                createdOrder.OrderItems = orderItems.ToList();
                await _orderRepository.CompleteAsync();

                var orderDto = _mapper.Map<OrderDto>(createdOrder);

                // Send notifications after successful transaction
                try
                {
                    if (!string.IsNullOrWhiteSpace(orderRequest.UserEmail))
                    {
                        await _notificationService.SendOrderConfirmationEmail(orderRequest.UserEmail, order.OrderNumber, order.TotalAmount);
                    }
                    await _notificationService.SendAdminNewOrderEmail(order.OrderNumber, order.TotalAmount, orderRequest.UserEmail, orderRequest.PhoneNumber);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "Failed to send one or more order notification emails for order {OrderNumber}", order.OrderNumber);
                }

                return new OkActionResponse<OrderDto>(orderDto, "Order created successfully.");
            }
            catch (Exception ex)
            {
                // Transaction rollback handled by repository
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

            // Validate Pakistani phone number format
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.PhoneNumber, @"^03\d{9}$"))
            {
                error = "Phone number must be 11 digits.";
                return false;
            }

            return true;
        }

        private bool ValidateGlassesAvailability(Glasses glasses, int quantity, out string error)
        {
            error = null;
            if (!glasses.IsActive)
            {
                error = "Order can't be placed for this glasses as it's inactive";
                return false;
            }
            if (glasses.AvailableQuantity < quantity)
            {
                error = "Order can't be placed for this product as available quantity is less than the requested quantity";
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
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
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
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
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
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
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
                var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
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

                var orderDto = _mapper.Map<OrderResponseDto>(order);

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

                var orderDto = _mapper.Map<OrderResponseDto>(order);
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
                if (order.OrderStatus == newStatus) return new FailActionResponse<OrderResponseDto>("New status is same as current status.");

                order.OrderStatus = newStatus;
                await _orderRepository.CompleteAsync();

                var orderDto = _mapper.Map<OrderResponseDto>(order);

                // send email when order becomes ready for pickup
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

                var userOrderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(userOrders);

                return new OkActionResponse<IEnumerable<OrderResponseDto>>(userOrderDtos);
            }
            catch (Exception)
            {
                return new FailActionResponse<IEnumerable<OrderResponseDto>>("An error occurred while retrieving the user's orders.");
            }
        }


    }
}
